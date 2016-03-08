using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Core;
using Fusion.Core.Utils;
using Fusion.Core.Mathematics;
using System.IO;

namespace ShooterDemo.Core {


	public class FXEvent {
		
		/// <summary>
		/// FX Event type.
		/// </summary>
		public FXEventType FXType;

		/// <summary>
		/// FX Event source position.
		/// Example for bullet trail - muzzle position.
		/// Example for explosion - explosion origin.
		/// </summary>
		public Vector3 Origin;

		/// <summary>
		/// FX Event target position.
		/// Example for bullet trail - hit position
		/// Example for explosion - direction where explosion grows.
		/// </summary>
		public Vector3 Target;

		/// <summary>
		/// FX Event normalized direction.
		/// Example for bullet trail - hit normal
		/// Example for explosion - not used.
		/// </summary>
		public Vector3 Normal;



		public FXEvent ()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="fxType"></param>
		/// <param name="position"></param>
		/// <param name="target"></param>
		/// <param name="orient"></param>
		public FXEvent ( FXEventType fxType, Vector3 origin, Vector3 target, Vector3 normal )
		{
			this.FXType		=	fxType;
			this.Origin		=	origin;
			this.Target		=	target;
			this.Normal		=	normal.Normalized();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write ( BinaryWriter writer )
		{
			writer.Write( (uint)FXType );
			writer.Write( Origin );
			writer.Write( Target );
			writer.Write( Normal );
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		public void Read ( BinaryReader reader )
		{
			FXType	=	(FXEventType)reader.ReadUInt32();
			Origin	=	reader.Read<Vector3>();
			Target	=	reader.Read<Vector3>();
			Normal	=	reader.Read<Vector3>();
		}
	}
}
