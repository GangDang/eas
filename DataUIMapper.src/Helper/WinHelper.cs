using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace EAS.Data
{
    class WinHelper
    {
        public static void SetDefaultControlProperty(Control control, ref MapperInfo mi)
        {
            if (control is TextBox)  //�ı���
            {
                mi.ControlProperty = "Text";
            }
            else if (control is RichTextBox)  //�ı�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is Label)  //�ı�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is LinkLabel)  //�ı�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is DateTimePicker)  //ʱ��ѡ��
            {
                mi.ControlProperty = "Value";
            }
            else if (control is ComboBox)  //�����б�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is CheckBox)  //��ѡ
            {
                mi.ControlProperty = "Checked";
            }
            else if (control is RadioButton)  //��ѡ
            {
                mi.ControlProperty = "Checked";
            }
            else if (control is ListBox)  //�б�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is NumericUpDown)  //��������
            {
                mi.ControlProperty = "Value";
            }
        }

        /// <summary>
        /// ��ⳣ�ÿؼ�
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool CheckCommonControl(IComponent component)
        {
            if (component is TextBox)  //TextBox
            {
                return true;
            }
            //else if (component is Label)  //Label
            //{
            //    return true;
            //}
            //else if (component is LinkLabel)  //LinkLabel
            //{
            //    return true;
            //}
            else if (component is ListBox)  //ListBox
            {
                return true;
            }
            else if (component is ComboBox)  //ComboBox
            {
                return true;
            }
            else if (component is RichTextBox)  //RichTextBox
            {
                return true;
            }
            else if (component is DateTimePicker)  //DateTimePicker
            {
                return true;
            }
            else if (component is NumericUpDown)  //NumericUpDown
            {
                return true;
            }
            else if (component is CheckBox)  //CheckBox
            {
                return true;
            }
            else if (component is RadioButton)  //RadioButton
            {
                return true;
            }

            return false;
        }
    }
}
