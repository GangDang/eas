using System;
using EAS.Security;
using System.ComponentModel;

namespace EAS.SilverlightClient.AddIn
{
	/// <summary>
	/// �Ѿ�ѡ����ʻ����߽�ɫ��
	/// </summary>
    public class SelectedPrivileger : INotifyPropertyChanged
	{
        bool m_Checked;
        string m_Name;
        int m_Value;
        PrivilegerType m_Type = PrivilegerType.Role;

        /// <summary>
        /// �Ƿ��Ѿ�ѡ��
        /// </summary>
        public bool Checked
        {
            get
            {
                return this.m_Checked;
            }
            set
            {
                this.m_Checked = value;
                this.NotifyPropertyChanged("Checked");
            }
        }

		/// <summary>
		/// ��¼ID/��ɫ���ơ�
		/// </summary>
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }
		/// <summary>
		/// �ʻ�Or��ɫ��
		/// </summary>
        public PrivilegerType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.NotifyPropertyChanged("Type");
            }
        }

		/// <summary>
		/// Ȩ�ޡ�
		/// </summary>
        public int Permissions
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
                this.NotifyPropertyChanged("Permissions");
            }
        }

        /// <summary>
        /// �˺Ż��߽�ɫ����
        /// </summary>
        public object Tag { get; set; }

		public void AppendPermissions(int permissions)
		{
            this.Permissions |= permissions;
		}

		public override string ToString()
		{
            return this.Name;
		}

        /// <summary>
        /// д��PropertyChanged�¼�֪ͨ��
        /// </summary>
        /// <param name="propertyName">�������ơ�</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged ��Ա

        /// <summary>
        /// �ڸ�������ֵʱ������
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
	}
}
