using System;
using System.Collections.Generic;
using System.Text;

namespace EAS.Data.Adapter
{
    /// <summary>
    /// ��������
    /// </summary>
    public interface IAdapter
    {
        /// <summary>
        /// ���ӵ��ؼ�������
        /// </summary>
        void Connect(DataUIMapper dataUIMapper, object controlsContainer);

        /// <summary>
        /// ����ָ���ؼ���
        /// </summary>
        object FindControl(string controlId);

        /// <summary>
        /// ���¶������Ե�UI��ʾ��
        /// </summary>
        void UpdateUI(DataUIMapper dm);

        /// <summary>
        /// ����UII��ʾ���������ԡ�
        /// </summary>
        void UpdateObject(DataUIMapper dm); 
    }

    public abstract class BaseAdapter : IAdapter
    {
        DataUIMapper _dm;

        /// <summary>
        /// DataUIMapper�����
        /// </summary>
        public DataUIMapper DataUIMapper
        {
            get
            {
                return this._dm;
            }
        }

        #region IAdapter ��Ա

        public virtual void Connect(DataUIMapper dataUIMapper, object controlsContainer)
        {
            this._dm = dataUIMapper;
            this._dm.Adapter = this;
        }

        public abstract object FindControl(string controlId);

        public abstract void UpdateUI(DataUIMapper dm);

        public abstract void UpdateObject(DataUIMapper dm);

        #endregion
    }
}
