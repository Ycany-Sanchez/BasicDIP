using System.Drawing;
using System.Drawing.Imaging;

namespace WinFormsApp1
{
    public sealed record CoinCountResult(
        int Count5C, int Count10C, int Count25C, int Count1P, int Count5P,
        double Total, Bitmap Annotated, string Summary);

    public static class CoinCounter
    {
        private static readonly double[] Values = { 0.05, 0.10, 0.25, 1.0, 5.0 };
        private static readonly string[] Names = { "5c", "10c", "25c", "P1", "P5" };

        /// <summary>Human-readable reject log from the last <see cref="CountCoins"/> call.</summary>
        public static IReadOnlyList<string> LastRejects { get; private set; } = Array.Empty<string>();

        public static CoinCountResult CountCoins(Bitmap source)
        {
            int w = source.Width, h = source.Height;
            byte[,] gray = ToGrayscale(source);
            int thr = OtsuThreshold(gray, w, h);
            bool[,] fg = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    fg[x, y] = gray[x, y] < thr;

            // Close small gaps/lettering via box close using integral image.
            // Radius adapts to image scale: big scans need wide close to
            // merge coin lettering, small dense shots need narrow to avoid
            // fusing neighboring coins into one blob.
            int closeR = Math.Clamp(Math.Min(w, h) / 220, 2, 7);
            // Pad with background so edge coins don't get pushed into the
            // image border by the close (the left 25c in coins.png sits ~2px
            // from the edge; dilate r=2 made it touch and get rejected).
            int pad = closeR + 6;
            int wp = w + pad * 2, hp = h + pad * 2;
            bool[,] fgPad = new bool[wp, hp];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    fgPad[x + pad, y + pad] = fg[x, y];
            bool[,] rawPad = (bool[,])fgPad.Clone();
            bool[,] closedPad = BoxClose(fgPad, wp, hp, closeR);
            // Fill holes for outer shape, but keep raw copy for hole detection.
            bool[,] filledPad = FillHoles(closedPad, wp, hp);

            var compsPad = Label(filledPad, closedPad, wp, hp);
            // Map back to original coords for annotation/rejects.
            var comps = new List<Comp>();
            foreach (var c in compsPad)
            {
                c.MinX -= pad; c.MinY -= pad; c.MaxX -= pad; c.MaxY -= pad;
                // Truly truncated only if raw coin pixels touch the photo edge.
                c.TouchesBorder = TouchesRawBorder(rawPad, wp, hp, pad, c);
                comps.Add(c);
            }
            // Drop specks, border artifacts and non-coin blobs (caption text:
            // tiny bbox fill ratio). Split merged touching coins via aspect.
            double imgArea = (double)w * h;
            var rejects = new List<string>();
            var blobs = new List<Comp>();
            foreach (var c in comps)
            {
                int bw0 = c.MaxX - c.MinX + 1, bh0 = c.MaxY - c.MinY + 1;
                if (c.FilledArea < imgArea * 0.0004) { rejects.Add($"speck area={c.FilledArea} bbox=({c.MinX},{c.MinY})-({c.MaxX},{c.MaxY})"); continue; }
                if (c.TouchesBorder) { rejects.Add($"border d~{c.Diameter:F0} bbox=({c.MinX},{c.MinY})-({c.MaxX},{c.MaxY})"); continue; }
                double fillRatio = c.FilledArea / (double)(bw0 * bh0);
                if (fillRatio < 0.55) { rejects.Add($"non-round fill={fillRatio:F2} area={c.FilledArea} bbox=({c.MinX},{c.MinY})-({c.MaxX},{c.MaxY})"); continue; }
                double aspect = Math.Max(bw0, bh0) / (double)Math.Min(bw0, bh0);
                if (aspect > 1.4)
                {
                    double single = Math.PI * Math.Pow(Math.Min(bw0, bh0) / 2.0, 2);
                    c.SplitCount = Math.Clamp((int)Math.Round(c.FilledArea / single), 2, 4);
                    c.DiameterOverride = Math.Sqrt(4.0 * (c.FilledArea / (double)c.SplitCount) / Math.PI);
                }
                blobs.Add(c);
            }
            LastRejects = rejects;
            if (blobs.Count == 0)
                return new CoinCountResult(0, 0, 0, 0, 0, 0, new Bitmap(source), "No coins found.");

            // One entry per coin (splits contribute N identical entries),
            // sorted by diameter, split into 5 groups at 4 largest gaps.
            var entries = new List<(Comp owner, double d)>();
            foreach (var c in blobs)
                for (int k = 0; k < Math.Max(1, c.SplitCount); k++)
                    entries.Add((c, c.DEff));
            entries.Sort((a, b) => a.d.CompareTo(b.d));
            int[] bounds = FindGroupBounds(entries.Select(e => e.d).ToArray(), 5);
            int[] counts = new int[5];
            for (int i = 0; i < entries.Count; i++)
            {
                int g = GroupOf(i, bounds);
                entries[i].owner.Group = g;
                counts[g]++;
            }

            // 5c cross-check: smallest group should hold the holed coins.
            // (Hole flag kept for summary; size drives the verdict per evenly-spaced spec.)
            int nCoins = entries.Count;
            double total = counts[0] * Values[0] + counts[1] * Values[1] + counts[2] * Values[2]
                + counts[3] * Values[3] + counts[4] * Values[4];

            Bitmap annotated = new Bitmap(source);
            using (var g = Graphics.FromImage(annotated))
            using (var pen = new Pen(Color.Red, Math.Max(2, w / 400)))
            using (var font = new Font("Arial", Math.Max(10, w / 60)))
            using (var brush = new SolidBrush(Color.Yellow))
            {
                foreach (var c in blobs)
                {
                    if (c.SplitCount > 1)
                    {
                        // Draw N side-by-side circles along the long axis.
                        int bw = c.MaxX - c.MinX + 1, bh = c.MaxY - c.MinY + 1;
                        bool vertical = bh >= bw;
                        double d = c.DEff;
                        double cx = (c.MinX + c.MaxX) / 2.0, cy = (c.MinY + c.MaxY) / 2.0;
                        for (int k = 0; k < c.SplitCount; k++)
                        {
                            double off = (k - (c.SplitCount - 1) / 2.0) * d;
                            float x = (float)(vertical ? cx - d / 2 : cx + off - d / 2);
                            float y = (float)(vertical ? cy + off - d / 2 : cy - d / 2);
                            g.DrawEllipse(pen, x, y, (float)d, (float)d);
                        }
                        g.DrawString(Names[c.Group] + "x" + c.SplitCount, font, brush, c.MinX, c.MinY);
                    }
                    else
                    {
                        g.DrawEllipse(pen, c.MinX, c.MinY, c.MaxX - c.MinX, c.MaxY - c.MinY);
                        g.DrawString(Names[c.Group], font, brush, c.MinX, c.MinY);
                    }
                }
            }

            string summary = $"5c:{counts[0]} 10c:{counts[1]} 25c:{counts[2]} P1:{counts[3]} P5:{counts[4]} = P{total:F2} (n={nCoins}, otsu={thr})";
            return new CoinCountResult(counts[0], counts[1], counts[2], counts[3], counts[4], total, annotated, summary);
        }

        private sealed class Comp
        {
            public int FilledArea; public int RawArea; public int MinX = int.MaxValue;
            public int MinY = int.MaxValue; public int MaxX; public int MaxY;
            public bool TouchesBorder; public int Group;
            public double Diameter => Math.Sqrt(4.0 * FilledArea / Math.PI);
            public double? DiameterOverride;
            public double DEff => DiameterOverride ?? Diameter;
            public bool HasHole => (FilledArea - RawArea) > FilledArea * 0.02;
            /// <summary>Set on original blobs merged from N coins.</summary>
            public int SplitCount;
        }

        private static byte[,] ToGrayscale(Bitmap bmp)
        {
            int w = bmp.Width, h = bmp.Height;
            var gray = new byte[w, h];
            var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            try
            {
                unsafe
                {
                    byte* base0 = (byte*)data.Scan0;
                    int stride = data.Stride;
                    for (int y = 0; y < h; y++)
                    {
                        byte* row = base0 + y * stride;
                        for (int x = 0; x < w; x++)
                        {
                            byte b = row[x * 3], g = row[x * 3 + 1], r = row[x * 3 + 2];
                            gray[x, y] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                        }
                    }
                }
            }
            finally { bmp.UnlockBits(data); }
            return gray;
        }

        private static int OtsuThreshold(byte[,] gray, int w, int h)
        {
            int[] hist = new int[256];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    hist[gray[x, y]]++;
            int total = w * h;
            double sum = 0;
            for (int i = 0; i < 256; i++) sum += (double)i * hist[i];
            double sumB = 0; int wB = 0; double best = -1; int thr = 128;
            for (int i = 0; i < 256; i++)
            {
                wB += hist[i];
                if (wB == 0) continue;
                int wF = total - wB;
                if (wF == 0) break;
                sumB += (double)i * hist[i];
                double mB = sumB / wB, mF = (sum - sumB) / wF;
                double between = (double)wB * wF * (mB - mF) * (mB - mF);
                if (between > best) { best = between; thr = i; }
            }
            return thr;
        }

        private static bool[,] BoxClose(bool[,] a, int w, int h, int r)
        {
            return BoxErode(BoxDilate(a, w, h, r), w, h, r);
        }

        private static bool[,] BoxDilate(bool[,] a, int w, int h, int r)
        {
            long[,] integ = Integral(a, w, h);
            var out_ = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    out_[x, y] = BoxSum(integ, w, h, x - r, y - r, x + r, y + r) > 0;
            return out_;
        }

        private static bool[,] BoxErode(bool[,] a, int w, int h, int r)
        {
            long[,] integ = Integral(a, w, h);
            var out_ = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int x0 = Math.Max(0, x - r), y0 = Math.Max(0, y - r);
                    int x1 = Math.Min(w - 1, x + r), y1 = Math.Min(h - 1, y + r);
                    long full = (long)(x1 - x0 + 1) * (y1 - y0 + 1);
                    out_[x, y] = BoxSum(integ, w, h, x0, y0, x1, y1) == full;
                }
            return out_;
        }

        private static long[,] Integral(bool[,] a, int w, int h)
        {
            var integ = new long[h + 1, w + 1];
            for (int y = 0; y < h; y++)
            {
                long row = 0;
                for (int x = 0; x < w; x++)
                {
                    row += a[x, y] ? 1 : 0;
                    integ[y + 1, x + 1] = integ[y, x + 1] + row;
                }
            }
            return integ;
        }

        private static long BoxSum(long[,] integ, int w, int h, int x0, int y0, int x1, int y1)
        {
            x0 = Math.Max(0, x0); y0 = Math.Max(0, y0);
            x1 = Math.Min(w - 1, x1); y1 = Math.Min(h - 1, y1);
            return integ[y1 + 1, x1 + 1] - integ[y0, x1 + 1] - integ[y1 + 1, x0] + integ[y0, x0];
        }

        private static bool[,] FillHoles(bool[,] fg, int w, int h)
        {
            // Flood background from borders; whatever is neither fg nor
            // background-connected is a hole -> fill it.
            var seen = new bool[w, h];
            var q = new Queue<(int, int)>();
            for (int x = 0; x < w; x++)
            {
                if (!fg[x, 0]) { seen[x, 0] = true; q.Enqueue((x, 0)); }
                if (!fg[x, h - 1]) { seen[x, h - 1] = true; q.Enqueue((x, h - 1)); }
            }
            for (int y = 0; y < h; y++)
            {
                if (!fg[0, y] && !seen[0, y]) { seen[0, y] = true; q.Enqueue((0, y)); }
                if (!fg[w - 1, y] && !seen[w - 1, y]) { seen[w - 1, y] = true; q.Enqueue((w - 1, y)); }
            }
            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
            while (q.Count > 0)
            {
                var (cx, cy) = q.Dequeue();
                for (int k = 0; k < 4; k++)
                {
                    int nx = cx + dx[k], ny = cy + dy[k];
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h || seen[nx, ny] || fg[nx, ny]) continue;
                    seen[nx, ny] = true;
                    q.Enqueue((nx, ny));
                }
            }
            var out_ = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    out_[x, y] = fg[x, y] || !seen[x, y];
            return out_;
        }

        private static bool TouchesRawBorder(bool[,] rawPad, int wp, int hp, int pad, Comp c)
        {
            // Blob bbox is in original coords; convert to padded for raw lookup.
            int minXp = c.MinX + pad, maxXp = c.MaxX + pad;
            int minYp = c.MinY + pad, maxYp = c.MaxY + pad;
            // Original photo edges in padded coords.
            int left = pad, right = pad + (wp - pad * 2) - 1;
            int top = pad, bottom = pad + (hp - pad * 2) - 1;
            if (minXp > left && maxXp < right && minYp > top && maxYp < bottom)
                return false;
            if (minXp <= left)
            {
                int y0 = Math.Max(minYp, top), y1 = Math.Min(maxYp, bottom);
                for (int y = y0; y <= y1; y++)
                    if (rawPad[left, y]) return true;
            }
            if (maxXp >= right)
            {
                int y0 = Math.Max(minYp, top), y1 = Math.Min(maxYp, bottom);
                for (int y = y0; y <= y1; y++)
                    if (rawPad[right, y]) return true;
            }
            if (minYp <= top)
            {
                int x0 = Math.Max(minXp, left), x1 = Math.Min(maxXp, right);
                for (int x = x0; x <= x1; x++)
                    if (rawPad[x, top]) return true;
            }
            if (maxYp >= bottom)
            {
                int x0 = Math.Max(minXp, left), x1 = Math.Min(maxXp, right);
                for (int x = x0; x <= x1; x++)
                    if (rawPad[x, bottom]) return true;
            }
            return false;
        }

        private static List<Comp> Label(bool[,] filled, bool[,] raw, int w, int h)
        {
            int[,] id = new int[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    id[x, y] = -1;
            var comps = new List<Comp>();
            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
            var q = new Queue<(int, int)>();
            for (int sy = 0; sy < h; sy++)
                for (int sx = 0; sx < w; sx++)
                {
                    if (!filled[sx, sy] || id[sx, sy] != -1) continue;
                    var c = new Comp();
                    int ci = comps.Count;
                    comps.Add(c);
                    q.Enqueue((sx, sy));
                    id[sx, sy] = ci;
                    while (q.Count > 0)
                    {
                        var (cx, cy) = q.Dequeue();
                        c.FilledArea++;
                        if (raw[cx, cy]) c.RawArea++;
                        if (cx < c.MinX) c.MinX = cx;
                        if (cy < c.MinY) c.MinY = cy;
                        if (cx > c.MaxX) c.MaxX = cx;
                        if (cy > c.MaxY) c.MaxY = cy;
                        if (cx == 0 || cy == 0 || cx == w - 1 || cy == h - 1) c.TouchesBorder = true;
                        for (int k = 0; k < 4; k++)
                        {
                            int nx = cx + dx[k], ny = cy + dy[k];
                            if (nx < 0 || ny < 0 || nx >= w || ny >= h || !filled[nx, ny] || id[nx, ny] != -1) continue;
                            id[nx, ny] = ci;
                            q.Enqueue((nx, ny));
                        }
                    }
                }
            return comps;
        }

        private static int[] FindGroupBounds(double[] sorted, int groups)
        {
            // Indices of the (groups-1) largest gaps between consecutive diameters.
            var gaps = new List<(double gap, int idx)>();
            for (int i = 0; i < sorted.Length - 1; i++)
                gaps.Add((sorted[i + 1] - sorted[i], i));
            gaps.Sort((a, b) => b.gap.CompareTo(a.gap));
            var bounds = gaps.Take(groups - 1).Select(g => g.idx).OrderBy(i => i).ToArray();
            return bounds;
        }

        private static int GroupOf(int sortedIndex, int[] bounds)
        {
            for (int g = 0; g < bounds.Length; g++)
                if (sortedIndex <= bounds[g]) return g;
            return bounds.Length;
        }
    }
}
