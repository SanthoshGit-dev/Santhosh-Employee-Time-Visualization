using System.IO.Compression;
using System.Text;
using EmployeeTimeVisualization.Models;

namespace EmployeeTimeVisualization.Presentation;

/// <summary>
/// Creates a dependency-free PNG pie chart. The chart includes a legend with
/// employee names and each employee's percentage of total recorded hours.
/// </summary>
public sealed class PieChartGenerator
{
    private const int Width = 1000;
    private const int Height = 650;
    private static readonly (byte R, byte G, byte B)[] Palette =
    {
        (52, 120, 198), (239, 145, 56), (75, 160, 120), (181, 92, 160),
        (220, 85, 85), (115, 125, 145), (205, 180, 65), (65, 170, 190),
        (150, 115, 205), (135, 155, 75)
    };

    public void Generate(IReadOnlyList<EmployeeSummary> employees, string outputPath)
    {
        var pixels = new byte[Width * Height * 3];
        for (var i = 0; i < pixels.Length; i += 3)
        {
            pixels[i] = 255; pixels[i + 1] = 255; pixels[i + 2] = 255;
        }

        var valid = employees.Where(e => e.TotalHours > 0).ToList();
        var total = valid.Sum(e => e.TotalHours);
        if (total > 0)
        {
            const int cx = 315, cy = 325, radius = 245;
            for (var y = cy - radius; y <= cy + radius; y++)
            for (var x = cx - radius; x <= cx + radius; x++)
            {
                var dx = x - cx; var dy = y - cy;
                if (dx * dx + dy * dy > radius * radius) continue;
                var angle = Math.Atan2(dy, dx);
                if (angle < -Math.PI / 2) angle += Math.PI * 2;
                var cumulative = -Math.PI / 2;
                for (var k = 0; k < valid.Count; k++)
                {
                    var sweep = valid[k].TotalHours / total * Math.PI * 2;
                    if (angle >= cumulative && angle < cumulative + sweep)
                    {
                        SetPixel(pixels, x, y, Palette[k % Palette.Length]);
                        break;
                    }
                    cumulative += sweep;
                }
            }
        }

        // Heading and legend text use a compact built-in 5x7 bitmap font.
        DrawText(pixels, 35, 25, "EMPLOYEE WORKING HOURS", 3, (25, 35, 50));
        DrawText(pixels, 610, 100, "EMPLOYEE BREAKDOWN", 2, (25, 35, 50));

        for (var i = 0; i < employees.Count; i++)
        {
            var y = 150 + i * 43;
            if (y + 35 >= Height) break;
            FillRect(pixels, 610, y, 24, 24, Palette[i % Palette.Length]);
            var name = employees[i].EmployeeName.ToUpperInvariant();
            if (name.Length > 22) name = name[..22];
            DrawText(pixels, 650, y, name, 2, (35, 45, 55));
            DrawText(pixels, 650, y + 17,
                $"{employees[i].PercentageOfTotal:0.0}%  ({employees[i].TotalHours:0.##} HRS)",
                1, (90, 95, 105));
        }

        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        WritePng(outputPath, pixels);
    }

    private static void SetPixel(byte[] p, int x, int y, (byte R, byte G, byte B) c)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return;
        var i = (y * Width + x) * 3;
        p[i] = c.R; p[i + 1] = c.G; p[i + 2] = c.B;
    }

    private static void FillRect(byte[] p, int x, int y, int w, int h, (byte R, byte G, byte B) c)
    {
        for (var yy = y; yy < y + h; yy++)
        for (var xx = x; xx < x + w; xx++) SetPixel(p, xx, yy, c);
    }

    private static readonly Dictionary<char, string[]> Font = new()
    {
        ['A'] = new[]{"01110","10001","10001","11111","10001","10001","10001"},
        ['B'] = new[]{"11110","10001","10001","11110","10001","10001","11110"},
        ['C'] = new[]{"01111","10000","10000","10000","10000","10000","01111"},
        ['D'] = new[]{"11110","10001","10001","10001","10001","10001","11110"},
        ['E'] = new[]{"11111","10000","10000","11110","10000","10000","11111"},
        ['F'] = new[]{"11111","10000","10000","11110","10000","10000","10000"},
        ['G'] = new[]{"01111","10000","10000","10111","10001","10001","01111"},
        ['H'] = new[]{"10001","10001","10001","11111","10001","10001","10001"},
        ['I'] = new[]{"11111","00100","00100","00100","00100","00100","11111"},
        ['J'] = new[]{"00111","00010","00010","00010","10010","10010","01100"},
        ['K'] = new[]{"10001","10010","10100","11000","10100","10010","10001"},
        ['L'] = new[]{"10000","10000","10000","10000","10000","10000","11111"},
        ['M'] = new[]{"10001","11011","10101","10101","10001","10001","10001"},
        ['N'] = new[]{"10001","11001","10101","10011","10001","10001","10001"},
        ['O'] = new[]{"01110","10001","10001","10001","10001","10001","01110"},
        ['P'] = new[]{"11110","10001","10001","11110","10000","10000","10000"},
        ['Q'] = new[]{"01110","10001","10001","10001","10101","10010","01101"},
        ['R'] = new[]{"11110","10001","10001","11110","10100","10010","10001"},
        ['S'] = new[]{"01111","10000","10000","01110","00001","00001","11110"},
        ['T'] = new[]{"11111","00100","00100","00100","00100","00100","00100"},
        ['U'] = new[]{"10001","10001","10001","10001","10001","10001","01110"},
        ['V'] = new[]{"10001","10001","10001","10001","10001","01010","00100"},
        ['W'] = new[]{"10001","10001","10001","10101","10101","10101","01010"},
        ['X'] = new[]{"10001","10001","01010","00100","01010","10001","10001"},
        ['Y'] = new[]{"10001","10001","01010","00100","00100","00100","00100"},
        ['Z'] = new[]{"11111","00001","00010","00100","01000","10000","11111"},
        ['0'] = new[]{"01110","10001","10011","10101","11001","10001","01110"},
        ['1'] = new[]{"00100","01100","00100","00100","00100","00100","01110"},
        ['2'] = new[]{"01110","10001","00001","00010","00100","01000","11111"},
        ['3'] = new[]{"11110","00001","00001","01110","00001","00001","11110"},
        ['4'] = new[]{"00010","00110","01010","10010","11111","00010","00010"},
        ['5'] = new[]{"11111","10000","10000","11110","00001","00001","11110"},
        ['6'] = new[]{"01110","10000","10000","11110","10001","10001","01110"},
        ['7'] = new[]{"11111","00001","00010","00100","01000","01000","01000"},
        ['8'] = new[]{"01110","10001","10001","01110","10001","10001","01110"},
        ['9'] = new[]{"01110","10001","10001","01111","00001","00001","01110"},
        ['%'] = new[]{"11001","11010","00100","01000","10110","00110","00000"},
        ['('] = new[]{"00010","00100","01000","01000","01000","00100","00010"},
        [')'] = new[]{"01000","00100","00010","00010","00010","00100","01000"},
        ['.'] = new[]{"00000","00000","00000","00000","00000","00110","00110"},
        [','] = new[]{"00000","00000","00000","00000","00110","00110","00100"},
        [':'] = new[]{"00000","00110","00110","00000","00110","00110","00000"},
        ['-'] = new[]{"00000","00000","00000","11111","00000","00000","00000"},
        [' '] = new[]{"00000","00000","00000","00000","00000","00000","00000"}
    };

    private static void DrawText(byte[] pixels, int x, int y, string text, int scale,
        (byte R, byte G, byte B) color)
    {
        var cursor = x;
        foreach (var ch in text)
        {
            if (Font.TryGetValue(ch, out var glyph))
            {
                for (var row = 0; row < 7; row++)
                for (var col = 0; col < 5; col++)
                    if (glyph[row][col] == '1')
                        FillRect(pixels, cursor + col * scale, y + row * scale, scale, scale, color);
            }
            cursor += 6 * scale;
            if (cursor > Width - 10) break;
        }
    }

    private static void WritePng(string path, byte[] rgb)
    {
        using var file = File.Create(path);
        file.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });
        using var header = new MemoryStream();
        WriteInt(header, Width); WriteInt(header, Height);
        header.WriteByte(8); header.WriteByte(2); header.WriteByte(0); header.WriteByte(0); header.WriteByte(0);
        WriteChunk(file, "IHDR", header.ToArray());

        using var raw = new MemoryStream();
        for (var y = 0; y < Height; y++)
        {
            raw.WriteByte(0); // PNG filter: None
            raw.Write(rgb, y * Width * 3, Width * 3);
        }
        using var compressed = new MemoryStream();
        using (var z = new ZLibStream(compressed, CompressionLevel.Optimal, true))
        {
            var rawBytes = raw.ToArray();
            z.Write(rawBytes, 0, rawBytes.Length);
        }
        WriteChunk(file, "IDAT", compressed.ToArray());
        WriteChunk(file, "IEND", Array.Empty<byte>());
    }

    private static void WriteInt(Stream s, int value)
    {
        s.WriteByte((byte)(value >> 24)); s.WriteByte((byte)(value >> 16));
        s.WriteByte((byte)(value >> 8)); s.WriteByte((byte)value);
    }

    private static void WriteChunk(Stream output, string type, byte[] data)
    {
        WriteInt(output, data.Length);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes); output.Write(data);
        var crcInput = typeBytes.Concat(data).ToArray();
        WriteInt(output, unchecked((int)Crc32(crcInput)));
    }

    private static uint Crc32(byte[] bytes)
    {
        uint crc = 0xffffffff;
        foreach (var b in bytes)
        {
            crc ^= b;
            for (var k = 0; k < 8; k++)
                crc = (crc & 1) != 0 ? (crc >> 1) ^ 0xedb88320 : crc >> 1;
        }
        return ~crc;
    }
}

internal static class StreamExtensions
{
    public static void CopyToStream(this ReadOnlySpan<byte> bytes, Stream destination)
        => destination.Write(bytes);
}
