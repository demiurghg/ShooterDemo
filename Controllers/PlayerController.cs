using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core;
using Fusion.Core.Content;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;
using Fusion.Engine.Input;
using Fusion.Engine.Client;
using Fusion.Engine.Server;
using Fusion.Engine.Graphics;
using ShooterDemo.Core;
using BEPUphysics;
using BEPUphysics.Character;


namespace ShooterDemo.Controllers {
	//class PlayerController : EntityController<object> {
		

	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="game"></param>
	//	/// <param name="space"></param>
	//	public PlayerController ( World world ) : base(world)
	//	{
	//		controllers = new Dictionary<uint,CharacterController>();
	//	}



	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="gameTime"></param>
	//	public void Update ( GameTime gameTime )
	//	{
	//		foreach ( var controller in controllers ) {
				
	//			var index = World.GetIndex( controller.Key );
				
	//			World.Entities[index].Position	=	MathConverter.Convert( controller.Value.Body.Position );
				
	//			//	Add control here from entities user command flags...
	//			//	...
	//		}
	//	}



	//	/// <summary>
	//	/// 
	//	/// </summary>
	//	/// <param name="id"></param>
	//	public void Kill ( uint id )
	//	{
			
	//	}

	//}
}
