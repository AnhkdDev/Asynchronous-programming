namespace Async_await
{
    internal class Program
    {
        static void DoSomeThing(int seconds, string msg, ConsoleColor color)
        {
            lock (Console.Out)// khóa console lại để các luồng khác không chiếm, thực hiện xong luồng này mới đến luồng khác
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{msg,10} ... Start");
                Console.ResetColor();
            }

            for (int i = 1; i <= seconds; i++)
            {
                lock (Console.Out)//ngăn nhiều luồng cùng lúc truy cập vào cugnf 1 tài nguyên dùng chung, tránh xung đột
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine($"{msg,10} {i,2}");
                    Console.ResetColor();
                    Thread.Sleep(1000);//khi dùng lệnh này cái luồng đang chạy sẽ dừng lại 1s
                }
            }
            lock (Console.Out)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{msg,10} ... End");
                Console.ResetColor();
            }

        }

        static Task Task2()
        {
            Task t2 = new Task(
                () =>
                {
                    DoSomeThing(10, "T2", ConsoleColor.Green);
                });
            t2.Start();
            return t2;
        }

        static Task Task3()
        {
            Task t3 = new Task(
              (object ob) =>
              {
                  string tentacvu = (string)ob;
                  DoSomeThing(4, tentacvu, ConsoleColor.Yellow);
              }, "T3");//tương đương biểu thức lambda (Object ob) => {}
            t3.Start();
            return t3;
        }

        static void Main(string[] args)
        {
            //synchronouns: lập trình đồng bộ
            //DoSomeThing(6, "T1", ConsoleColor.Magenta);
            //DoSomeThing(10, "T2", ConsoleColor.Green );
            //DoSomeThing(4, "T3", ConsoleColor.Yellow);
            //Console.WriteLine("hello world");
            //=> khi thực thi thì nó sẽ làm từng bước 1: T1 hoàn thành => T2 => T3


            //asynchronous: lập trình "BẤT" đồng bộ
            //Task, Task<> dùng để biểu diễn 1 tác vụ
            //Task
            Task t2 = new Task(
                () =>
                {
                    DoSomeThing(10, "T2", ConsoleColor.Green);
                });

            Task t3 = new Task(
                (object ob) =>
                {
                    string tentacvu = (string)ob;
                    DoSomeThing(4, tentacvu, ConsoleColor.Yellow);
                }, "T3"); //tương đương biểu thức lambda (Object ob) => {}

            t2.Start();//chạy trên các thread khác nhau
            t3.Start();//chạy trên các thread khác nhau
            DoSomeThing(6, "T1", ConsoleColor.Magenta);//chạy trên các thread khác nhau

            //DoSomeThing(10, "T2", ConsoleColor.Green);
            //DoSomeThing(4, "T3", ConsoleColor.Yellow);

            //t2.Wait();//phương thức này đảm bảo t2 hoàn thành trước khi qua các cái khác
            //t3.Wait();

            Task.WaitAll(t2, t3);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
