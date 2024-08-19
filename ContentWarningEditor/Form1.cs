using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Win32;


namespace ContentWarningEditor
{
    public partial class Form1 : Form
    {
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"..\LocalLow\Landfall Games\Content Warning\Saves\metaData.m");
        public Form1()
        {
            InitializeComponent();
            
        }

        void updateFace()
        {
            if (textBox1.Text.Length > 0)
            {
                RegistryKey currentUserKey = Registry.CurrentUser;
                RegistryKey faceTextKey = currentUserKey.OpenSubKey("Software\\Landfall Games\\Content Warning", true);
                byte[] bytes = ConvertHexStringToByteArray(this.textBox1.Text);
                faceTextKey.SetValue("FaceText_h3883740665", bytes, RegistryValueKind.Binary);
                
                faceTextKey.Close();
            }
        }
        static byte[] ConvertHexStringToByteArray(string hex)
        {
            string[] hexValuesSplit = hex.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[hexValuesSplit.Length];

            for (int i = 0; i < hexValuesSplit.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexValuesSplit[i], 16);
            }

            return bytes;
        }

        void updateMeatCoins()
        {
            string json = File.ReadAllText(filePath);
            // Находим индекс "MetaCoins"
            int index = json.IndexOf("\"MetaCoins\":");

            if (index == -1)
                return ; // Если не найдено, возвращаем оригинальный JSON

            // Находим конец значения
            int startIndex = index + "\"MetaCoins\":".Length;
            int endIndex = json.IndexOf(",", startIndex);

            // Если запятая не найдена, ищем конец объекта
            if (endIndex == -1)
            {
                endIndex = json.IndexOf("}", startIndex);
            }

            // Формируем новое значение
            string newMetaCoins = this.textBox2.Text;

            // Заменяем старое значение на новое
            json = json.Substring(0, startIndex) + newMetaCoins + json.Substring(endIndex);
            if (textBox2.Text.Length > 0)
            {
                File.WriteAllText(filePath, json);

            }
            


        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("Content Warning").Any())
            {
                if (MessageBox.Show("Игра не должна быть запущена, Закрыть игру?", "Ошибка", MessageBoxButtons.YesNo) ==
                    DialogResult.Yes)
                {
                    foreach (Process proc in Process.GetProcessesByName("Content Warning"))
                    {
                        proc.Kill();
                    }
                    updateMeatCoins();
                    updateFace();
                    MessageBox.Show("Изменения внесены успешно");
                    return;
                }
                MessageBox.Show("Ошибка внесения изменений");
                
            }
            else
            {
                updateMeatCoins();
                updateFace();
                MessageBox.Show("Изменения внесены успешно");
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Проверяем, является ли введённый символ цифрой
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Запрещаем ввод символов, кроме цифр
                e.Handled = true;
            }
        }
    }
}
