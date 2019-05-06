using System;
using EAS.Data.ORM;
using EAS.Sessions;

namespace EAS.Explorer
{
    /// <summary>
    /// ϵͳ�˺Žӿڡ�
    /// </summary>
    public interface IAccount : IDataEntity, IClient
    {
        /// <summary>
        /// ��ȡ�ʻ���Ӧ��ԭʼ����ı�ʶ������Ա���Ĺ���֤�š�
        /// </summary>
        string OriginalID
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ��¼ID����󳤶�Ϊ 64 ���ַ���
        /// </summary>
        string LoginID
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ�ʻ���������Ϣ��
        /// </summary>
        /// <remarks>��ֵ����Ӧ����Ϣϵͳȷ����ʹ�á�</remarks>
        int Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ�ʻ��Ѿ���¼Ӧ�ó���Ĵ�����
        /// </summary>
        long LoginCount
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ�ʻ������ƣ���󳤶�Ϊ 64 ���ַ���
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡ���ʻ���������Ϣ��
        /// </summary>
        string Description
        {
            get;
            set;
        }

        /// <summary>
        /// ���ڻ���/���š�
        /// </summary>
        IOrganization Organization
        {
            get;
            set;
        }

        /// <summary>
        /// ��ȡϵͳ�˺ŵ�Ŀ¼�����˺����͡�
        /// </summary>
        Guid Certificate
        {
            get;
            set;
        }

        /// <summary>
        /// ��������֤�顣
        /// </summary>
        bool EnableCertificate { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�û��Ƿ����ߡ�
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�û��Ƿ��Ѿ���������
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// ��ȡһ��ֵ����ֵָʾ��ǰ�û��Ƿ��Ѿ������á�
        /// </summary>
        bool IsDisabled { get; }
    }
}
