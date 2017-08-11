using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// Recipe170_静的メソッドのクラス名を省略したい
using static System.Console;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;

namespace Recipe
{
    //Recipe_169_拡張メソッド定義
    static class ExtInt
    {
        internal static int Abs(this int n)
        {
            return n < 0 ? -1 * n : n;
        }
    }

    [DataContract]  // for Recipe265 オブジェクトとJSONを相互に変換
    partial class Rikishi : IComparable<Rikishi>
    {
        [DataMember(Name = "Name")]
        public string Name { get; private set; }
        [DataMember(Name = "rank")]
        private string rank;
        [DataMember(Name = "Age")]
        public int Age { get; set; } = 30;

        public Rikishi(string name, string rank)
        {
            Name = name;
            this.rank = rank;
        }
        public override string ToString()
        {
            return $"{rank} {Name}関";
        }

        public int CompareTo(Rikishi you)
        {
            var d = new Dictionary<string, int>()
            {
                ["横綱"] = 10,
                ["大関"] = 9,
                ["関脇"] = 8,
                ["小結"] = 7,
                ["前頭"] = 6,
            };
            var mine = d[rank];
            var yours = d[you.rank];
            return mine - yours;
        }

        // デリゲート宣言
        public delegate int Functions(int x, int y);

        // デリゲート変数
        public Functions Funcs { get; set; }
        // イベント変数(デリゲート変数をイベント指定した，と考えればよい)
        public event Functions events;
        public event Functions Events
        {
            add
            {
                events += value;
            }
            remove
            {
                events -= value;
            }
        }
        public void submit(int x, int y)
        {
            //クラス内でどちらも実行できる
            Funcs(x, y);
            events(x, y);
        }
    }
    static class OuterRikisi
    {
        static public void ActionFromOutside(Rikishi r)
        {
            // デリゲート変数は外部からでも実行できる
            r.Funcs(0, 1);

            // イベント変数は外部からでも実行できない
            //r.events(0, 1);  => NG
        }
    }

    class Program
    {
        void Recipe030_数字を文字列にする()
        {
            WriteLine(1234.ToString("#,#"));
            WriteLine(1234.ToString("#,#"));
            WriteLine(123456789.ToString("#,#"));
            WriteLine(1234.ToString("00000"));
        }
        void Recipe031_文字列を数字にする()
        {
            WriteLine(int.Parse("1234"));
            WriteLine(int.Parse("1,234", NumberStyles.Number));
            WriteLine(long.Parse("DEAFBEAF", NumberStyles.HexNumber));
        }
        void Recipe032_文字列や数値を日付データにする()
        {
            {
                var d = DateTime.Parse("2017-07-30");
                WriteLine(d);
            }
            {
                var d = DateTime.ParseExact("20170730", "yyyyMMdd", null);
                WriteLine(d);
            }
        }
        void Recipe033_日付データから文字列や数値を取得する()
        {
            var dt = new DateTime(2017, 7, 30, 11, 30, 59);
            WriteLine(dt.ToString("yyyy"));
            WriteLine(dt.ToString("yyyy-mm-dd"));
        }
        void Reipe034_現在時刻を取得する()
        {
            var now = DateTime.Now;
            var today = DateTime.Today;
            WriteLine(now);
            WriteLine(today);
        }
        void Recipe046_桁あふれを検出する()
        {
            var x = 123456789;
            var y = 123456789;
            try
            {
                checked
                {
                    WriteLine(x * y);
                }
            }
            catch (OverflowException)
            {
                WriteLine("桁あふれを検出");
            }
        }
        void Recipe058_配列_コレクションの要素を順番に処理する()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            WriteLine(array.Sum());
            WriteLine(array.Where((element, i) => i % 2 == 0).Sum());
            WriteLine(array.Aggregate((acc, next) => { return acc * next; }));
        }
        void Recipe063_型を調べる()
        {
            object str = "abc";
            WriteLine(str is object);
            WriteLine(str is string);
            WriteLine(str is char[]);

        }
        void Recipe064_例外なしにcastする()
        {
            object str = "abc";
            WriteLine(str as string);
            WriteLine(str as char[]);
        }
        void Recipe065_型オブジェクトを取得する()
        {
            var strType = typeof(string);
            strType.GetMethods().Select((m => m.Name)).Distinct().OrderBy((s => s)).ToList().ForEach((method) =>
            {
                WriteLine(method);
            });
        }
        void Recipe066_nullならば既定値を入れる()
        {
            object a = null;
            object b = null;
            object c = "hello";

            object d = a ?? b ?? "default";
            object e = a ?? b ?? c ?? "default";
            WriteLine(d);
            WriteLine(c);
        }
        void Recipe070_文字列内の文字数を調べる()
        {
            WriteLine("日本語".Length); // => 3
            WriteLine("🐟".Length);    // => 2 ２文字で１文字を表すサロゲートペアは注意!!
        }
        void Recipe075_改行やバックスラッシュを含んだ文字列を定義する()
        {
            var str = @"C:\Users\example\Documents";
            var query = @"
select col1
      ,col2
from
       table
where
       col3 = @param
";
            WriteLine(str);
            WriteLine(query);

        }
        void Recipe076_補完文字列を作る()
        {
            var a = 1234;
            var s = $"文字列に{a}が組み込まれる．";
            WriteLine(s);
        }
        void Recipe087_正規表現を利用してマッチングする()
        {
            var sikona = @"
白鵬 日馬富士 稀勢の里 鶴竜 照ノ富士 豪栄道 高安 玉鷲 御嶽海 嘉風 琴奨菊 正代
 貴景勝 栃ノ心 北勝富士 勢 遠藤 宇良 輝 千代翔馬 栃煌山 逸ノ城 阿武咲 貴ノ岩
 大栄翔 碧山 石浦 徳勝龍 隠岐の海 千代大龍 松鳳山 千代の国 大翔丸 荒鷲 豪風 宝富士
 蒼国来 佐田の海 琴勇輝 錦木 千代丸 臥牙丸";
            {
                var reg = new Regex(@"(稀勢)の里");
                WriteLine(reg.IsMatch(sikona));

                var m = reg.Match(sikona);
                WriteLine(m.Value);     // => 稀勢の里
                WriteLine(m.Groups[0]); // => 稀勢の里
                WriteLine(m.Groups[1]); // => 稀勢
            }

            {
                var reg = new Regex(@"(\S{1,2})富士");

                foreach (var i in reg.Matches(sikona))
                {
                    WriteLine(i);
                }
            }
        }
        void Recipe098_配列内のコピーを作成する()
        {
            var array = new string[] { "白鵬", "日馬富士", "稀勢の里" };
            var newarray = array.Clone() as string[];
            Array.ForEach(newarray, e => WriteLine(e));
        }
        void Recipe099_配列内のデータを移動する()
        {
            var array = new string[] { "白鵬", "日馬富士", "稀勢の里" };
            Array.Copy(array, 1, array, 0, array.Length - 1);
            array[array.Length - 1] = "白鵬";
            Array.ForEach(array, e => WriteLine(e));
        }
        void Recipe155_ジェネリックメソッドを定義したい()
        {
            WriteLine(GenericMax(5, 4, 3));
            var kotosyogiku = new Rikishi("琴奨菊", "大関");
            var hakuho = new Rikishi("白鵬", "横綱");
            var mitakeumi = new Rikishi("御嶽海", "前頭");
            WriteLine(GenericMax(kotosyogiku, mitakeumi, hakuho));
        }
        void Recipe161_引数を利用して値を返すメソッド(int a, int b, out int sum)
        {
            //var n = sum; ng
            sum = a + b; // 設定せずにreturnするとコンパイルエラー
        }
        void Recipe162_呼び出しに利用された引数を更新するメソッド(int a, int b, ref int sum)
        {
            var augend = sum;     // out と異なりOK
            sum = augend + a + b; // 更新
        }
        void Recipe169_拡張メソッド定義()
        {
            int n = -5;
            var abs = n.Abs(); // => 5;
            WriteLine(abs);
        }
        void Recipe172_nullかもしれないオブジェクトにアクセス()
        {
            string x = null;
            var y = new List<string>();
            int[] a = null;
            WriteLine(x?.Length); // => null
            WriteLine(y.Find(s => s.Length > 0)?.ToString()); // => null
            WriteLine(a?[3]); // => null
        }
        int div(int dividend, int divisor)
        {
            return dividend / divisor;
        }
        void Recipe173_名前付き引数呼び出し()
        {
            // 宣言と引数の順序を入れ替えできる．
            WriteLine(div(divisor: 5, dividend: 10)); // =>2 
        }
        void Recipe265_オブジェクトとJSONを相互に変換()
        {
            var rikishi = new Rikishi("琴奨菊", "大関");
            rikishi.Age = 28;
            var serializer = new DataContractJsonSerializer(typeof(Rikishi));
            using (var mem = new MemoryStream())
            {
                serializer.WriteObject(mem, rikishi);
                WriteLine(Encoding.UTF8.GetString(mem.ToArray()));
            }
        }
        public static async Task<DateTime> getDateTimeAsync()
        {
            var t = await Task.Run(() =>
            {
                Thread.Sleep(5000);
                return DateTime.Now;
            });
            return t;
        }
        async void Recipe283_非同期処理()
        {
            var result = await getDateTimeAsync();
            Console.WriteLine(result);
        }
        void Recipe297_プロパティ名を指定してプロパティにアクセス()
        {
            var rikishi = new Rikishi("琴奨菊", "大関");
            var nameProperty = typeof(Rikishi).GetProperty("Name");
            var name = nameProperty.GetValue(rikishi);
            WriteLine(name);
        }
        void Recipe299_フィールド名を指定してフィールドにアクセス()
        {
            var rikishi = new Rikishi("琴奨菊", "大関");
            var rankField = typeof(Rikishi).GetField("rank", BindingFlags.NonPublic | BindingFlags.Instance);
            rankField.SetValue(rikishi, "前頭");
            WriteLine(rikishi);
        }

        void Recipe300_変数名やメソッド名を文字列で取得()
        {
            var someVar = "変数";
            var msg = $"{nameof(someVar)}に，値[{someVar}]を格納.";
            WriteLine(msg);
        }
        void RecipeA57_バックスラッシュを意識せずにパスを組み立てる()
        {
            var path = Path.Combine(@"C:\Program Files (x86)", @"Microsoft Visual Studio 14.0");
            WriteLine(path);
        }

        /// <summary>
        /// Recipe A26 ユーティリティクラスを定義する．
        /// </summary>
        static class Utility
        {
        }

        // Recipe155_ジェネリックメソッドを定義
        static T GenericMax<T>(T x, T y, T z) where T : IComparable<T>
        {
            List<T> a = new List<T> { x, y, z };
            return a.Max();
        }

        class Custom
        {
            static string sname;
            string name;

            public Custom()
            {
                this.name = sname;
            }
            public Custom(string name)
            {
                this.name = name;
            }
            /// <summary>
            /// Recipe プロパティを定義
            /// </summary>
            public string Field
            {
                set
                {
                    name = value;
                }
                get
                {
                    return "always unknown";
                }
            }


            /// <summary>
            /// Recipe130静的コンストラクタ
            /// </summary>
            static Custom()
            {
                sname = "undefined";
            }
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Recipe283_非同期処理();
            ReadKey();
        }
    }

}