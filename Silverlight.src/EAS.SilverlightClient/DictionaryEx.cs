using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;

namespace EAS.SilverlightClient
{
    [Serializable()]
    class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// ��ʼ�� DictionaryEx �����ʵ����
        /// </summary>
        public DictionaryEx():base()
        {

        }

        /// <summary>
        /// ��ʼ�� DictionaryEx �����ʵ������ʵ��������ָ���� IDictionary��
        /// </summary>
        /// <param name="dictionary">IDictionary������Ԫ�ر����Ƶ��µ� DictionaryEx��</param>
        ///<exception cref="System.ArgumentNullException">dictionary Ϊ null��</exception>
        public DictionaryEx(IDictionary<TKey, TValue> dictionary):base(dictionary)
        {

        }

        /// <summary>
        /// ��ʼ�� DictionaryEx�����ʵ������ʹ��ָ����IEqualityComparer��
        /// </summary>
        /// <param name="comparer">�Ƚϼ�ʱҪʹ�õ� IEqualityComparer ʵ�֣�����Ϊ null���Ա�Ϊ������ʹ��Ĭ�ϵ�IEqualityComparer��</param>
        public DictionaryEx(IEqualityComparer<TKey> comparer):base(comparer)
        {

        }

        /// <summary>
        /// ��ʼ�� DictionaryEx �����ʵ������ʵ��Ϊ���Ҿ���ָ���ĳ�ʼ������
        /// </summary>
        /// <param name="capacity">DictionaryEx �ɰ����ĳ�ʼԪ������</param>
        public DictionaryEx(int capacity):base(capacity)
        {

        }

        /// <summary>
        /// ��ʼ�� DictionaryEx �����ʵ������ʵ��������ָ���� IDictionary�и��Ƶ�Ԫ�ز�ʹ��ָ���� IEqualityComparer��
        /// </summary>
        /// <param name="dictionary">IDictionary,����Ԫ�ر����Ƶ��µ� DictionaryEx��</param>
        /// <param name="comparer">�Ƚϼ�ʱҪʹ�õ� IEqualityComparer ʵ�֡�</param>
        ///<exception cref="System.ArgumentNullException">dictionary Ϊ null��</exception> 
        public DictionaryEx(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer):base(dictionary,comparer)
        {

        }

        /// <summary>
        /// ��ʼ�� DictionaryEx �����ʵ������ʵ��Ϊ���Ҿ���ָ���ĳ�ʼ��������ʹ��ָ���� IEqualityComparer��
        /// </summary>
        /// <param name="capacity">DictionaryEx�ɰ����ĳ�ʼԪ������</param>
        /// <param name="comparer">�Ƚϼ�ʱҪʹ�õ� IEqualityComparerʵ�֡�</param>
        ///<exception cref="System.ArgumentNullException">dictionary Ϊ null��</exception> 
        public DictionaryEx(int capacity, IEqualityComparer<TKey> comparer):base(capacity,comparer)
        {

        }

        /// <summary>
        /// �� DictionaryEx ���Ƴ���ָ��������ֵ��
        /// </summary>
        /// <param name="index">����λ�á�</param>
        /// <returns>����ɹ��ҵ����Ƴ���Ԫ�أ���Ϊ true������Ϊ false�� ����� Dictionary��û���ҵ� key/Value���˷����򷵻� false��</returns>
        public bool Remove(int index)
        {
            if ((index >= this.Keys.Count) | (index < 0))
                throw new ArgumentOutOfRangeException("index");

            return this.Remove(this.GetKey(index));
        }

        /// <summary>
        /// ��ȡ������ָ����������Ԫ�ء�
        /// </summary>
        /// <param name="index">Ҫ��û����õ�Ԫ�ش��㿪ʼ��������</param>
        /// <exception cref="System.ArgumentOutOfRangeException">index С�� 0��- �� -index ���ڻ�����������ȡ�</exception> 
        /// <returns>ָ����������Ԫ�ء�</returns>
        public TValue this[int index]
        {
            get
            {
                if ((index >= this.Keys.Count) |(index <0))
                    throw new ArgumentOutOfRangeException("index");
 
                return base[this.GetKey(index)];
            }
            set
            {
                if ((index >= this.Keys.Count) |(index <0))
                    throw new ArgumentOutOfRangeException("index");

                base[this.GetKey(index)] = value;
            }
        }

        /// <summary>
        /// ������������ֵ��
        /// </summary>
        /// <param name="index">����λ�á�</param>
        /// <returns>ֵ��</returns>
        public TKey GetKey(int index)
        {
            if ((index >= this.Keys.Count) |(index <0))
                throw new ArgumentOutOfRangeException("index");

            IEnumerator<TKey> enumerator = this.Keys.GetEnumerator();
            enumerator.Reset();
            int i= 0;
            while (enumerator.MoveNext())
            {
                if(i==index)
                    return enumerator.Current;

                i++;
            }

            return default(TKey);
        }
    }
}
