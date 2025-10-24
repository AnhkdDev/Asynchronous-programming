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

        static async Task Task2()
        {
            Task t2 = new Task(
                () =>
                {
                    DoSomeThing(10, "T2", ConsoleColor.Green);
                });
            t2.Start();
            await t2;//tương đương như wait, đảm bảo những chỉ thị phía sau await chỉ đc thực hiện sau khi tast t2 đã hoàn thành
            //AWAIT SẼ TRẢ VỀ TAST LUÔN NÊN K CẦN RETURN
            //khi nó trả về như vậy thì nó sẽ không khóa đi threat chính
            //t2.Wait();
            Console.WriteLine("T2 da hoan thanh");
        }

        static async Task Task3()
        {
            Task t3 = new Task(
              (object ob) =>
              {
                  string tentacvu = (string)ob;
                  DoSomeThing(4, tentacvu, ConsoleColor.Yellow);
              }, "T3");//tương đương biểu thức lambda (Object ob) => {}
            t3.Start();
            await t3;
            Console.WriteLine("T3 da hoan thanh");
        }

        //async/await
        //cách biến đổi 1 phương thức void bth thành 1 phương thức bất đồng bộ: xóa void thêm async Task và bên trong nó phải có await
        static async Task Main1(string[] args)
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
            Task t2 = Task2();
            Task t3 = Task3();

            //t2.Start();//chạy trên các thread khác nhau
            //t3.Start();//chạy trên các thread khác nhau
            DoSomeThing(6, "T1", ConsoleColor.Magenta);//chạy trên các thread khác nhau

            //DoSomeThing(10, "T2", ConsoleColor.Green);
            //DoSomeThing(4, "T3", ConsoleColor.Yellow);

            //t2.Wait();//phương thức này đảm bảo t2 hoàn thành trước khi qua các cái khác
            //t3.Wait();

            //Task.WaitAll(t2, t3);
            await t2;
            await t3;

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static async Task<string> Task4()
        {
            Task<string> t4 = new Task<string>(
                () =>
                {
                    DoSomeThing(10, "T4", ConsoleColor.Green);
                    return "Return from T4";
                }); // () => {return string;} ( là 1 Func<string> )
            t4.Start();
            var kq = await t4;//await trả về giá trị khi t4 hoàn thành
            Console.WriteLine("T4 da hoan thanh");
            return kq;
        }

        static async Task<string> Task5()
        {
            Task<string> t5 = new Task<string>(
                (object ob) =>
                {
                    string t = (string)ob;
                    DoSomeThing(4, t, ConsoleColor.Yellow);
                    return $"Return from {t}";
                }, "T5"); // (object ob) => {return string;}  ( là 1 Func<object, string>, object )
            t5.Start();
            var kq = await t5;
            Console.WriteLine("T5 da hoan thanh");
            return kq;
        }

        static async Task Main(string[] args)
        {
            // sử dụng Task khi hoàn thành có giá trị trả về
            // Task<T>

            Task<string> t4 = Task4();
            Task<string> t5 = Task5();

            DoSomeThing(6, "T1", ConsoleColor.Magenta);

            var kq4 = await t4;
            var kq5 = await t5;

            Console.WriteLine(kq4);
            Console.WriteLine(kq5);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
    }
}
