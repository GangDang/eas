using System;
using System.Collections.Generic;
using System.Text;

namespace fyiReporting.RdlDesign
{
    /// <summary>
    /// ��ӡֽ�Žṹ���Թ��Ƽ��㡣
    /// </summary>
    struct PrintPage
    {
        public PrintPage(string name,decimal width,decimal height)
        {
            this.Name = name;
            this.Width = width;
            this.Height = height;
        }
        /// <summary>
        /// ����
        /// </summary>
        public string Name;

        /// <summary>
        /// ���
        /// </summary>
        public decimal Width;

        /// <summary>
        /// �߶�
        /// </summary>
        public decimal Height;

        /// <summary>
        /// ���
        /// </summary>
        public string WidthString
        {
            get
            {
                return this.Width.ToString() + "mm";
            }
        }

        /// <summary>
        /// �߶�
        /// </summary>
        public string HeightString
        {
            get
            {
                return this.Height.ToString() + "mm";
            }
        }
    }
}
