using System;
using System.Net.Http;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dotnethttp
{
  class Program
  {
    static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
      Console.WriteLine("Current Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);

      var watch = Stopwatch.StartNew();
      try
      {
        foreach (var i in Enumerable.Range(0, 30))
        {
          HttpResponseMessage response = await client.GetAsync("http://localhost:5000/weatherforecast");

          Console.WriteLine("Current Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);

          response.EnsureSuccessStatusCode();

          string responseBody = await response.Content.ReadAsStringAsync();
        }
      }
      catch (HttpRequestException e)
      {
        Console.WriteLine("Exception Caught: {0}", e.Message);
      }
      watch.Stop();
      Console.WriteLine("Sequential requests timing: {0}ms", watch.ElapsedMilliseconds);

      watch = Stopwatch.StartNew();

      var requests = Enumerable.Range(0, 30)
        .Select(i =>
        {
          Console.WriteLine("Current Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);

          return client.GetAsync("http://localhost:5000/weatherforecast");
        })
        .ToArray();

      Console.WriteLine("Current Thread ID: {0}", Thread.CurrentThread.ManagedThreadId);
      
      await Task.WhenAll(requests);

      watch.Stop();
      Console.WriteLine("Asynchronous requests timing: {0}ms", watch.ElapsedMilliseconds);
    }
  }
}
