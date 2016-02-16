using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Engine.Common;
using System.IO;

namespace ShooterDemo.Core {
	public abstract class Processor {

		/// <summary>
		/// Initialized game system
		/// </summary>
		public abstract void Initialize ();

		/// <summary>
		/// Updates game system
		/// </summary>
		/// <param name="gameTime"></param>
		public abstract void Update ( GameTime gameTime );

		/// <summary>
		/// Writes system's state.
		/// </summary>
		/// <param name="writer"></param>
		public abstract void WriteState ( BinaryWriter writer );

		/// <summary>
		/// Reads system's state.
		/// </summary>
		/// <param name="reader"></param>
		public abstract void ReadState ( BinaryReader reader );

		/// <summary>
		/// Called on each game systems when entity is removed.
		/// </summary>
		/// <param name="entity"></param>
		public abstract void Remove ( Entity entity );
	}
}
