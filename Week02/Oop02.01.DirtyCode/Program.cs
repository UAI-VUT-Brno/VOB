using System;
using System.Collections.Generic;

namespace DirtyCode
{
    public class s1
    {
        public string n = "";
        public int c = 40;
        public string t = "Student";
        public double[] g = new double[10];
        public int gc = 0;
        public bool a = true;
        public string StudentStudentNote = "";
        public int tmp = 0;
        public int tmp2 = 0;
        public string LastOperation = "";
    }

    public static class DATA
    {
        public static List<s1> list = new List<s1>();
        public static int counter = 0;
        public static string LastError = "";
        public static bool flag = false;
    }

    //////////////////////////////////////////////////////////////////////////
    // 12.9. JF - added grades
    // 3.10. JF - fixed the report, do not touch the 0.15 !!
    // 7.10. PK - commented out the export, we will need it later
    //////////////////////////////////////////////////////////////////////////

    class Program
    {
        static void Main()
        {
            Add("Novak", 40, "Student");
            Add("Svoboda", 12, "student");
            Add("Kral", 0, "PhD");
            Add("", 30, "Student");

            AddGrade("Novak", 1);
            AddGrade("Novak", 2);
            AddGrade("Svoboda", 3);
            AddGrade("Svoboda", 4);
            AddGrade("Kral", 1);
            AddGrade("Neznamy", 2);

            DoEverything();

            Console.WriteLine();
            Console.WriteLine("done, errors: " + DATA.LastError);
        }

        public static void Add(string n, int c, string t)
        {
            s1 x = new s1();
            x.n = n;
            x.c = c;
            x.t = t;
            DATA.list.Add(x);
            DATA.counter = DATA.counter + 1;
        }

        public static void AddGrade(string n, double val)
        {
            try
            {
                int i = Find(n);
                s1 x = DATA.list[i];

                if (x.gc < 10)
                {
                    x.g[x.gc] = val;
                    x.gc = x.gc + 1;
                    x.LastOperation = "grade";
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("NOT FOUND"))
                {
                    DATA.LastError = DATA.LastError + "missing:" + n + ";";
                }
            }
        }

        public static int Find(string n)
        {
            for (int i = 0; i < DATA.list.Count; i++)
            {
                if (DATA.list[i].n == n)
                {
                    return i;
                }
            }

            throw new Exception("NOT FOUND");
        }

        // public static void Export(string path)
        // {
        //     var w = new System.IO.StreamWriter(path);
        //     for (int i = 0; i < DATA.list.Count; i++)
        //     {
        //         w.WriteLine(DATA.list[i].n + ";" + DATA.list[i].c);
        //     }
        //     w.Close();
        // }

        public static void DoEverything()
        {
            double total = 0;
            int cnt = 0;
            string report = "";
            double bonus = 0;

            for (int i = 0; i < DATA.list.Count; i++)
            {
                s1 x = DATA.list[i];

                if (x != null)
                {
                    if (x.a == true)
                    {
                        if (x.n != "")
                        {
                            if (x.t == "Student" || x.t == "student" || x.t == "STUDENT")
                            {
                                if (x.c >= 40)
                                {
                                    bonus = 0.15;

                                    if (x.gc > 0)
                                    {
                                        double s = 0;
                                        for (int j = 0; j < x.gc; j++)
                                        {
                                            s = s + x.g[j];
                                        }

                                        double a = s / x.gc;

                                        if (a <= 1.5)
                                        {
                                            report = report + x.n + " excellent " + (a + bonus) + "\n";
                                        }
                                        else if (a <= 2.5)
                                        {
                                            report = report + x.n + " good " + (a + bonus) + "\n";
                                        }
                                        else if (a <= 3.5)
                                        {
                                            report = report + x.n + " ok " + (a + bonus) + "\n";
                                        }
                                        else
                                        {
                                            report = report + x.n + " bad " + (a + bonus) + "\n";
                                        }

                                        total = total + a;
                                        cnt = cnt + 1;
                                    }
                                    else
                                    {
                                        report = report + x.n + " no grades\n";
                                    }
                                }
                                else
                                {
                                    report = report + x.n + " not enough credits (" + x.c + "/40)\n";
                                }
                            }
                            else
                            {
                                if (x.t == "PhD")
                                {
                                    report = report + x.n + " phd student\n";
                                }
                                else
                                {
                                    report = report + x.n + " unknown type\n";
                                }
                            }
                        }
                    }
                }
            }

            try
            {
                double avg = total / cnt;
                Console.WriteLine("average: " + avg);
            }
            catch (Exception)
            {
            }

            Console.WriteLine(report);

            for (int i = 0; i < DATA.list.Count; i++)
            {
                s1 x = DATA.list[i];
                if (x.t == "Student" || x.t == "student" || x.t == "STUDENT")
                {
                    if (x.c >= 40)
                    {
                        Mail(x.n);
                    }
                }
            }
        }

        public static void Mail(string n)
        {
            if (DATA.flag == false)
            {
                Console.WriteLine("mail to " + n + ": you passed");
            }
        }

        public static void do_report_old(s1 x)
        {
            Console.WriteLine(x.n + " " + x.c + " " + x.t);
        }
    }
}
