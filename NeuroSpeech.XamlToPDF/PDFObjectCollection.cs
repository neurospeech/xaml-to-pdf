using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NeuroSpeech.XamlToPDF
{

	public class PDFChildObject : PDFObject {



		public PDFObject Parent {
			get {
				return GetValue<PDFObject>("Parent");
			}
			set {
				SetValue<PDFObject>("Parent", value);
			}
		}
	}


	public class PDFObjectCollection<T> : PDFObject, IList<T>, IPDFObjectCollection
		where T:PDFChildObject
	{

		#region public PDFPage Create()
		public TX Create<TX>()
			where TX:T
		{
			TX item = Document.CreateObject<TX>();
			item.Parent = this;
			this.Items.Add(item);
			return item;
		}
		#endregion


		#region protected override void  WriteProperties(TextWriter writer)
		protected override void WriteProperties(TextWriter writer)
		{
			base.WriteProperties(writer);
			if (this.Count > 0)
			{
				writer.WriteLine("/Kids [{0}]", string.Join(" ", this.Select(x => x.Ref)));
			}
			writer.WriteLine("/Count {0}", this.Count);
		}
		#endregion

		#region List<T> Implementation
		List<T> Items = new List<T>();

		#region public int  IndexOf(T item)
		public int IndexOf(T item)
		{
			return Items.IndexOf(item);
		}
		#endregion


		#region public void  Insert(int index, T item)
		public void Insert(int index, T item)
		{
			Items.Insert(index, item);
		}
		#endregion


		#region public void  RemoveAt(int index)
		public void RemoveAt(int index)
		{
			Items.RemoveAt(index);
		}
		#endregion


		public T this[int index]
		{
			get
			{
				return Items[index];
			}
			set
			{
				Items[index] = value;
			}
		}

		#region public void  Add(T item)
		public void Add(T item)
		{
			Items.Add(item);
		}
		#endregion


		#region public void  Clear()
		public void Clear()
		{
			Items.Clear();
		}
		#endregion


		#region public bool  Contains(T item)
		public bool Contains(T item)
		{
			return Items.Contains(item);
		}
		#endregion


		#region public void  CopyTo(T[] array, int arrayIndex)
		public void CopyTo(T[] array, int arrayIndex)
		{
			Items.CopyTo(array, arrayIndex);
		}
		#endregion


		public int Count
		{
			get { return Items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		#region public bool  Remove(T item)
		public bool Remove(T item)
		{
			return Items.Remove(item);
		}
		#endregion


		#region public IEnumerator<T>  GetEnumerator()
		public IEnumerator<T> GetEnumerator()
		{
			return Items.GetEnumerator();
		}
		#endregion


		#region System.Collections.IEnumerator  System.Collections.IEnumerable.GetEnumerator()
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}
		#endregion
		#endregion

	}
}
