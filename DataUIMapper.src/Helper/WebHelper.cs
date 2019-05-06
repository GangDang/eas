using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace EAS.Data
{
    class WebHelper
    {
        public static void SetDefaultControlProperty(Control control, ref MapperInfo mi)
        {
            if (control is TextBox)  //�ı���
            {
                mi.ControlProperty = "Text";
            }
            else if (control is Label)  //�ı�
            {
                mi.ControlProperty = "Text";
            }
            else if (control is Calendar)  //Calendar
            {
                mi.ControlProperty = "SelectedDate";
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
            else if (component is ListBox)  //ListBox
            {
                return true;
            }
            else if (component is Calendar)  //Calendar
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
