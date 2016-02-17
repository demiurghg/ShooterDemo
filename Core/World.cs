using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using System.IO;

namespace ShooterDemo.Core {
	public class World {

		/// <summary>
		/// Gets aggregator factory.
		/// </summary>
		public Factory Factory {
			get; private set;
		}


		/// <summary>
		/// Adds processor to aggregator
		/// </summary>
		/// <param name="processor"></param>
		public void Add ( Processor processor )
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// Gets processor
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>()
		{
			throw new NotImplementedException();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		/// <param name="world"></param>
		/// <returns></returns>
		public Entity Spawn ( string prefab, Entity parent, Matrix world )
		{
			throw new NotImplementedException();
		}



		/// <summary>
		/// Updates all processors in order of addition.
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update ( GameTime gameTime )
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void WriteState ( BinaryWriter writer )
		{
			throw new NotImplementedException();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void ReadState ( BinaryReader reader )
		{
			throw new NotImplementedException();
		}
	}
}
