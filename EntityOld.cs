using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Mathematics;
using Fusion.Core.Configuration;
using Fusion.Core.Shell;
using Fusion.Core.Utils;
using Fusion.Engine.Common;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using Fusion.Engine.Audio;
using Fusion.Engine.Input;
using System.IO;

namespace ShooterDemo {
	public abstract class EntityOld {

		//	public fields :
		public int ID { get; private set; }
		public byte TypeID { get; private set; }


		/// <summary>
		/// 
		/// </summary>
		public EntityOld ()
		{
			//	assign unique server side id and type id.
			ID		= Interlocked.Increment( ref atomicCounter );
			TypeID 	= GetTypeID(this.GetType());
		}


		abstract public void Activate ( GameWorld gameWorld );
		abstract public void Deactivate ( GameWorld gameWorld );
		abstract public void Update ( GameTime gameTime );
		abstract public void Read ( BinaryReader reader );
		abstract public void Write ( BinaryWriter writer );


		/*-----------------------------------------------------------------------------------------
		 *	Static stuff :
		-----------------------------------------------------------------------------------------*/

		//	static fields :
		static int atomicCounter;
		static KeyValuePair<string,Type>[] Types;


		/// <summary>
		/// Search assembly for game entity classes and add them to dicitionary.
		/// </summary>
		static EntityOld()
		{
			Types	=	Misc.GetAllSubclassesOf( typeof(EntityOld), false )
							.Select( t => new KeyValuePair<string,Type>( t.Name, t ) )
							.ToArray();

			Log.Message("{0} game entity types detected", Types.Length);

			if (Types.Length>byte.MaxValue) {
				throw new InvalidOperationException("Too much game entity types");
			}
		}



		/// <summary>
		/// Gets type ID
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		static byte GetTypeID ( Type type )
		{
			for ( byte id = 0; id<(byte)Types.Length; id++ ) {
				if (Types[id].Value==type) {
					return id;
				}
			}

			throw new ArgumentException("Bad entity type: {0}", type.ToString() );
		}



		/// <summary>
		/// Server side entity creation
		/// </summary>
		/// <param name="typeId"></param>
		/// <returns></returns>
		public static EntityOld Spawn ( byte typeId )
		{
			return (EntityOld)Activator.CreateInstance( Types[typeId].Value );
		}



		/// <summary>
		/// Client side entity creation
		/// </summary>
		/// <param name="typeId"></param>
		/// <returns></returns>
		public static EntityOld Replicate ( byte typeId, int id )
		{
			var ent = (EntityOld)Activator.CreateInstance( Types[typeId].Value );
			ent.ID = id;
			return ent;
		}

	}
}
