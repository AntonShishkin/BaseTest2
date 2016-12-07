using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 



    public class Platezh
    {

        public Platezh(long ind1, DateTime Date, string Name, decimal Pay)
        {

            this.Date = Date;
            this.Name = Name;
            this.Pay = Pay;
            this.ind1 = ind1;

            SDate = String.Format("{0:dd.MM.yyy}", Date);//Для замены внешнего представления даты
        }

        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal Pay { get; set; }

        public long ind1 { get; set;}
        public string SDate { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<Platezh> result = new List<Platezh>();//Список основной
        public List<Platezh> resultF = new List<Platezh>();//Список на экран

        public void addList(Platezh plat)
        {
            result.Add(plat);
            grid.ItemsSource = resultF;
        }

        public MainWindow()
        {
            InitializeComponent();

            //Установка дней на элементах
            DateTime DMon = DateTime.Now;
            DMon = DMon.AddDays(-DMon.Day + 1);
            DMon = DMon.AddHours(-DMon.Hour);
            DMon = DMon.AddMinutes(-DMon.Minute);

            DateTime DNow = DateTime.Now;
            DNow = DNow.AddHours(23 - DNow.Hour);
            DNow = DNow.AddMinutes(59 - DNow.Minute);

            DateTime DNow1 = DateTime.Now;
            DNow1 = DNow1.AddHours(- DNow1.Hour);
            DNow1 = DNow1.AddMinutes(- DNow1.Minute);

            DateT1.SelectedDate = DNow;
            DateF1.SelectedDate = DMon;
            DateF2.SelectedDate = DNow1;

            Loaded += strLoad;
            Loaded += DataFil;
            Loaded += grid_Loaded;
        }

        public void strLoad(object sender, RoutedEventArgs e)
        {
            //Подгрузка из файла
            FileStream FStr = new FileStream(@"1.txt", FileMode.Open);
            StreamReader Read = new StreamReader(FStr);
            
            string[] temp1;
            string[] temp = Read.ReadToEnd().Split('\n');
            for (int a = 0; a < temp.Length; a++)
            {
                if (temp[a]=="")
                {
                    continue;
                }
                temp1 = temp[a].Split('+');
                result.Add(new Platezh(Convert.ToInt64(temp1[0]),Convert.ToDateTime(temp1[1]), temp1[2], Convert.ToDecimal(temp1[3])));
            }
            
            Read.Close();
            FStr.Close();
        }

        public void grid_Loaded(object sender, RoutedEventArgs e)
        {
            //Первоначальное формирование таблицы
            grid.AutoGenerateColumns = false;
            grid.CanUserSortColumns = false;

            grid.ItemsSource = resultF;
            grid.Items.Refresh();

            grid.SelectedIndex = 0;

        }

        private void DataFil(object sender, RoutedEventArgs e)
        {//При закрытии календаря
            DateFilter();
        }

        private void DateFilter()
        {//Фильтр по дате-имени
            resultF.RemoveRange(0, (int)resultF.LongCount());

            for (int i1 = 0; i1 < (int)result.LongCount(); i1++)
            {
                DateFilter2(result[i1]);
            }
        }

        private void DateFilter2(Platezh p)
        {//Продолжение фильтра
            int srMin = DateTime.Compare(p.Date, Convert.ToDateTime(DateF1.SelectedDate));
            int srMax = DateTime.Compare(p.Date, Convert.ToDateTime(DateF2.SelectedDate));
            if (srMin >= 0 && srMax <= 0 && p.Name.Contains(NameF.Text))
            {
                resultF.Add(p);
            }

            grid.AutoGenerateColumns = false;
            grid.CanUserSortColumns = false;

            grid.ItemsSource = resultF;
            grid.Items.Refresh();

            decimal SuM = 0;
            foreach (Platezh j in resultF)
            {
                SuM += j.Pay;
            }
            Summ.Content = SuM;
        }


        private void Butt1_Click(object sender, RoutedEventArgs e)
        {//Ввод платежа
            try
            {
                Convert.ToDecimal(Tb2.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Сумма платежа введена не корректно");
                return;
            }

            if (Tb1.Text == "")
            {
                MessageBox.Show("Введите имя клиента");
                return;
            }

            Platezh PLN = new Platezh(result.LongCount(),Convert.ToDateTime(DateT1.SelectedDate), Tb1.Text, Convert.ToDecimal(Tb2.Text));

            result.Add(PLN);

            DateFilter2(PLN);
        }

        private void Butt2_Click(object sender, RoutedEventArgs e)
        {//Удаление платежа
            int nums=grid.SelectedIndex ;
            try
            {
                long ind2=resultF[nums].ind1;
                bool trig = false;
                for (int i = 0; i < (int)result.LongCount(); i++)
                {
                    if (result[i].ind1==ind2)
                    {
                        result.RemoveAt(i);
                        trig = true;
                        continue;
                    }
                    if (!trig) result[i].ind1 = i;
                    else result[i].ind1 = i - 1;

                }
                
                trig = false;

                    resultF.RemoveAt(nums);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
                grid.Items.Refresh();
                if (nums>0)
                    grid.SelectedIndex = nums - 1;
                else
                    grid.SelectedIndex = nums;

                decimal SuM = 0;
                foreach (Platezh j in resultF)
                {
                    SuM += j.Pay;
                }
                Summ.Content = SuM;
            
        }

        private void ButtSave_Click(object sender, RoutedEventArgs e)
        {//Сохранение в внешний файл
            StreamWriter Write = new StreamWriter(@"1.txt");

            foreach (Platezh a1 in result)
            {
                Write.WriteLine(a1.ind1+"+"+a1.Date + "+" +
                a1.Name + "+" + a1.Pay);
            }
            Write.Close();
            grid.Items.Refresh();

        }

        private void DateF1_SelectedDateChanged_1(object sender, SelectionChangedEventArgs e)
        {//Изменение выбранной даты
            DateFilter();
        }

        private void NameF_TextChanged_1(object sender, TextChangedEventArgs e)
        {//Изменение фильтра по имени
            DateFilter();
        }

        private void ButtCh_Click(object sender, RoutedEventArgs e)
        {//Редактирование платежа
            try
            {
                Convert.ToDecimal(Tb2.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Сумма платежа введена не корректно");
                return;
            }

            if (Tb1.Text == "")
            {
                MessageBox.Show("Введите имя клиента");
                return;
            }

            Platezh PLN = new Platezh(result.LongCount(), Convert.ToDateTime(DateT1.SelectedDate), Tb1.Text, Convert.ToDecimal(Tb2.Text));

            try
            {
                int nums = grid.SelectedIndex;


                long ind2 = resultF[nums].ind1;
                for (int i = 0; i < (int)result.LongCount(); i++)
                {
                    if (result[i].ind1 == ind2)
                    {
                        result[i] = PLN;
                        break;
                    }
                }

                DateFilter2(PLN);

                grid.ItemsSource = resultF;
                grid.Items.Refresh();
            }
            catch (ArgumentOutOfRangeException)
            {

                return;
            }
        }


    }
}
