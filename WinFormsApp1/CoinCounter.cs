using System.Drawing;
using System.Drawing.Imaging;

namespace WinFormsApp1
{
    public sealed record CoinCountResult(
        int Count5C, int Count10C, int Count25C, int Count1P, int Count5P,
        double Total, Bitmap Annotated, string Summary);

    public static class CoinCounter
    {
        static readonly double[] Values = { 0.05, 0.10, 0.25, 1.0, 5.0 };
        static readonly string[] Names = { "5c", "10c", "25c", "P1", "P5" };

        public static IReadOnlyList<string> LastRejects { get; private set; } = Array.Empty<string>();

        public static CoinCountResult CountCoins(Bitmap src)
        {
            int w = src.Width, h = src.Height;
            byte[,] g = Gray(src, w, h);
            int thr = Otsu(g, w, h);

            // Foreground, padded so the close can't push edge coins into the border.
            int r = Math.Clamp(Math.Min(w, h) / 220, 2, 7);
            int pad = r + 6, wp = w + pad * 2, hp = h + pad * 2;
            var fg = new bool[wp, hp];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    fg[x + pad, y + pad] = g[x, y] < thr;
            var raw = (bool[,])fg.Clone();

            var comps = Label(Fill(Close(fg, wp, hp, r), wp, hp), wp, hp);
            foreach (var c in comps)
            {
                c.MinX -= pad; c.MinY -= pad; c.MaxX -= pad; c.MaxY -= pad;
                c.Edge = Cut(raw, w, h, pad, c);
            }

            double area = (double)w * h;
            var rejects = new List<string>();
            var blobs = new List<Blob>();
            foreach (var c in comps)
            {
                int bw = c.MaxX - c.MinX + 1, bh = c.MaxY - c.MinY + 1;
                if (c.Area < area * 0.0004) { rejects.Add($"speck {c.Area} [{c.MinX},{c.MinY}-{c.MaxX},{c.MaxY}]"); continue; }
                if (c.Edge) { rejects.Add($"border d~{c.D:F0} [{c.MinX},{c.MinY}-{c.MaxX},{c.MaxY}]"); continue; }
                if (c.Area / (double)(bw * bh) < 0.55) { rejects.Add($"flat [{c.MinX},{c.MinY}-{c.MaxX},{c.MaxY}]"); continue; }
                if (Math.Max(bw, bh) / (double)Math.Min(bw, bh) > 1.4)
                {
                    double single = Math.PI * Math.Pow(Math.Min(bw, bh) / 2.0, 2);
                    c.N = Math.Clamp((int)Math.Round(c.Area / single), 2, 4);
                    c.D = Math.Sqrt(4 * (c.Area / (double)c.N) / Math.PI);
                }
                blobs.Add(c);
            }
            LastRejects = rejects;
            if (blobs.Count == 0)
                return new CoinCountResult(0, 0, 0, 0, 0, 0, new Bitmap(src), "No coins found.");

            // One row per coin, sorted by size, cut at the 4 biggest gaps.
            var ds = blobs.SelectMany(b => Enumerable.Repeat((b, b.D), Math.Max(1, b.N))).OrderBy(e => e.Item2).ToList();
            var cuts = ds.Zip(ds.Skip(1), (a, b) => b.Item2 - a.Item2)
                .Select((gap, i) => (gap, i)).OrderByDescending(t => t.gap).Take(4).Select(t => t.i).OrderBy(i => i).ToArray();
            int[] counts = new int[5];
            for (int i = 0; i < ds.Count; i++)
            {
                int grp = Array.FindIndex(cuts, c => i <= c);
                ds[i].b.Group = grp < 0 ? 4 : grp;
                counts[ds[i].b.Group]++;
            }
            double total = counts.Select((n, i) => n * Values[i]).Sum();

            var out_ = new Bitmap(src);
            using (var gx = Graphics.FromImage(out_))
            using (var pen = new Pen(Color.Red, Math.Max(2, w / 400)))
            using (var font = new Font("Arial", Math.Max(10, w / 60)))
            using (var br = new SolidBrush(Color.Yellow))
                foreach (var c in blobs)
                {
                    if (c.N > 1)
                    {
                        bool vert = (c.MaxY - c.MinY) >= (c.MaxX - c.MinX);
                        double cx = (c.MinX + c.MaxX) / 2.0, cy = (c.MinY + c.MaxY) / 2.0;
                        for (int k = 0; k < c.N; k++)
                        {
                            double off = (k - (c.N - 1) / 2.0) * c.D;
                            float x = (float)(vert ? cx - c.D / 2 : cx + off - c.D / 2);
                            float y = (float)(vert ? cy + off - c.D / 2 : cy - c.D / 2);
                            gx.DrawEllipse(pen, x, y, (float)c.D, (float)c.D);
                        }
                        gx.DrawString(Names[c.Group] + "x" + c.N, font, br, c.MinX, c.MinY);
                    }
                    else
                    {
                        gx.DrawEllipse(pen, c.MinX, c.MinY, c.MaxX - c.MinX, c.MaxY - c.MinY);
                        gx.DrawString(Names[c.Group], font, br, c.MinX, c.MinY);
                    }
                }

            string s = $"5c:{counts[0]} 10c:{counts[1]} 25c:{counts[2]} P1:{counts[3]} P5:{counts[4]} = P{total:F2} (n={ds.Count}, otsu={thr})";
            return new CoinCountResult(counts[0], counts[1], counts[2], counts[3], counts[4], total, out_, s);
        }

        sealed class Blob
        {
            public int Area, MinX = int.MaxValue, MinY = int.MaxValue, MaxX, MaxY, Group, N;
            public bool Edge;
            public double D;
        }

        static byte[,] Gray(Bitmap bmp, int w, int h)
        {
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

        static int Otsu(byte[,] gray, int w, int h)
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

        static bool[,] Close(bool[,] a, int w, int h, int r)
        {
            long[,] integ(bool[,] s)
            {
                var t = new long[h + 1, w + 1];
                for (int y = 0; y < h; y++)
                {
                    long row = 0;
                    for (int x = 0; x < w; x++) { row += s[x, y] ? 1 : 0; t[y + 1, x + 1] = t[y, x + 1] + row; }
                }
                return t;
            }
            long sum(long[,] t, int x0, int y0, int x1, int y1)
            {
                x0 = Math.Max(0, x0); y0 = Math.Max(0, y0); x1 = Math.Min(w - 1, x1); y1 = Math.Min(h - 1, y1);
                return t[y1 + 1, x1 + 1] - t[y0, x1 + 1] - t[y1 + 1, x0] + t[y0, x0];
            }
            var di = integ(a);
            var d = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    d[x, y] = sum(di, x - r, y - r, x + r, y + r) > 0;
            var ei = integ(d);
            var e = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int x0 = Math.Max(0, x - r), y0 = Math.Max(0, y - r), x1 = Math.Min(w - 1, x + r), y1 = Math.Min(h - 1, y + r);
                    e[x, y] = sum(ei, x0, y0, x1, y1) == (long)(x1 - x0 + 1) * (y1 - y0 + 1);
                }
            return e;
        }

        static bool[,] Fill(bool[,] fg, int w, int h)
        {
            var seen = new bool[w, h];
            var q = new Queue<(int, int)>();
            void push(int x, int y) { if (!fg[x, y] && !seen[x, y]) { seen[x, y] = true; q.Enqueue((x, y)); } }
            for (int x = 0; x < w; x++) { push(x, 0); push(x, h - 1); }
            for (int y = 0; y < h; y++) { push(0, y); push(w - 1, y); }
            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
            while (q.Count > 0)
            {
                var (cx, cy) = q.Dequeue();
                for (int k = 0; k < 4; k++)
                {
                    int nx = cx + dx[k], ny = cy + dy[k];
                    if ((uint)nx < (uint)w && (uint)ny < (uint)h && !seen[nx, ny] && !fg[nx, ny]) { seen[nx, ny] = true; q.Enqueue((nx, ny)); }
                }
            }
            var o = new bool[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    o[x, y] = fg[x, y] || !seen[x, y];
            return o;
        }

        static bool Cut(bool[,] raw, int w, int h, int pad, Blob c)
        {
            // True only if pre-close pixels touch the photo edge inside the bbox.
            if (c.MinX > 0 && c.MaxX < w - 1 && c.MinY > 0 && c.MaxY < h - 1) return false;
            if (c.MinX <= 0) for (int y = Math.Max(0, c.MinY); y <= Math.Min(h - 1, c.MaxY); y++) if (raw[pad, y + pad]) return true;
            if (c.MaxX >= w - 1) for (int y = Math.Max(0, c.MinY); y <= Math.Min(h - 1, c.MaxY); y++) if (raw[pad + w - 1, y + pad]) return true;
            if (c.MinY <= 0) for (int x = Math.Max(0, c.MinX); x <= Math.Min(w - 1, c.MaxX); x++) if (raw[x + pad, pad]) return true;
            if (c.MaxY >= h - 1) for (int x = Math.Max(0, c.MinX); x <= Math.Min(w - 1, c.MaxX); x++) if (raw[x + pad, pad + h - 1]) return true;
            return false;
        }

        static List<Blob> Label(bool[,] f, int w, int h)
        {
            var id = new int[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    id[x, y] = -1;
            var out_ = new List<Blob>();
            var q = new Queue<(int, int)>();
            int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
            for (int sy = 0; sy < h; sy++)
                for (int sx = 0; sx < w; sx++)
                {
                    if (!f[sx, sy] || id[sx, sy] != -1) continue;
                    var c = new Blob();
                    id[sx, sy] = out_.Count;
                    out_.Add(c);
                    q.Enqueue((sx, sy));
                    while (q.Count > 0)
                    {
                        var (cx, cy) = q.Dequeue();
                        c.Area++;
                        if (cx < c.MinX) c.MinX = cx;
                        if (cy < c.MinY) c.MinY = cy;
                        if (cx > c.MaxX) c.MaxX = cx;
                        if (cy > c.MaxY) c.MaxY = cy;
                        for (int k = 0; k < 4; k++)
                        {
                            int nx = cx + dx[k], ny = cy + dy[k];
                            if ((uint)nx < (uint)w && (uint)ny < (uint)h && f[nx, ny] && id[nx, ny] == -1) { id[nx, ny] = id[cx, cy]; q.Enqueue((nx, ny)); }
                        }
                    }
                    c.D = Math.Sqrt(4.0 * c.Area / Math.PI);
                }
            return out_;
        }
    }
}
