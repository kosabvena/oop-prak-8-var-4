using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static int[] delays = { 500, 800, 300, 1000 };

    static string Work(int id)
    {
        Thread.Sleep(delays[id]);
        if (id == 1 || id == 3) throw new Exception($"Помилка в задачі {id + 1}");
        return $"Задача {id + 1} готова";
    }

    static async Task<string> WorkAsync(int id)
    {
        await Task.Delay(delays[id]);
        if (id == 1 || id == 3) throw new Exception($"Помилка в задачі {id + 1}");
        return $"Задача {id + 1} готова";
    }

    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("Послідовно:");
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < delays.Length; i++)
        {
            try { Console.WriteLine(Work(i)); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        long seq = sw.ElapsedMilliseconds;
        Console.WriteLine($"Час: {seq} мс\n");

        Console.WriteLine("Асинхронно:");
        sw.Restart();
        var tasks = Enumerable.Range(0, delays.Length).Select(WorkAsync).ToList();
        Task all = Task.WhenAll(tasks);

        try
        {
            await all;
        }
        catch
        {
            foreach (var ex in all.Exception!.InnerExceptions)
                Console.WriteLine(ex.Message);
        }

        foreach (var t in tasks.Where(t => t.IsCompletedSuccessfully))
            Console.WriteLine(t.Result);

        long asyn = sw.ElapsedMilliseconds;
        Console.WriteLine($"Час: {asyn} мс\n");

        Console.WriteLine($"Прискорення: {(double)seq / asyn:F2}x");
    }
}