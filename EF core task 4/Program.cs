using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EF_core_task_4.Entites;

namespace EF_core_task_4;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // عشان العربي يطبع صح في الكونسول
        Console.WriteLine("جاري الاتصال بالداتابيز وقراءة البيانات المبدئية...");

        using (var context = new BankDbContext())
        {
            // بنسحب الفروع ومعاها جدول المديرين المرتبط بيها باستخدام Include
            var branches = context.Branches
                                  .Include(b => b.Manager)
                                  .ToList();

            Console.WriteLine("\n====================================================");
            Console.WriteLine("        الفروع والمديرين المغذين في النظام         ");
            Console.WriteLine("====================================================");

            foreach (var branch in branches)
            {
                string managerName = branch.Manager != null
                    ? $"{branch.Manager.FirstName} {branch.Manager.LastName} (رقم قومي: {branch.Manager.NationalId})"
                    : "لا يوجد مدير حالياً للفرع";

                Console.WriteLine($"الفرع: {branch.Name} | كود: {branch.Code}");
                Console.WriteLine($"العنوان: {branch.Address}");
                Console.WriteLine($"المدير المسئول: {managerName}");
                Console.WriteLine("----------------------------------------------------");
            }
        }

        Console.WriteLine("\nتمت العملية بنجاح! اضغط أي زرار لقفل البرنامج...");
        Console.ReadKey();
    }
}