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
	public class Characters : EntityController {

		Dictionary<uint, CharacterController> controllers; 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public Characters ( World world, Space space ) : base(world)
		{
			controllers = new Dictionary<uint,CharacterController>();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( GameTime gameTime )
		{
			foreach ( var controller in controllers ) {
				
				var index = World.GetIndex( controller.Key );
				
				World.Entities[index].Position	=	MathConverter.Convert( controller.Value.Body.Position );
				
				//	Add control here from entities user command flags...
				//	...
			}
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			throw new NotImplementedException();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="entity"></param>
		public void AddCharacter ( Entity entity, 
			float height = 1.7f, 
			float crouchingHeight = 1.19f, 
			float radius = 0.6f, 
			float margin = 0.1f, 
			float mass = 10f, 
			float maximumTractionSlope = 0.8f, 
			float maximumSupportSlope = 1.3f, 
			float standingSpeed = 8f, 
			float crouchingSpeed = 3f, 
			float tractionForce = 1000f, 
			float slidingSpeed = 6f, 
			float slidingForce = 50f, 
			float airSpeed = 1f, 
			float airForce = 250f, 
			float jumpSpeed = 4.5f, 
			float slidingJumpSpeed = 3f, 
			float maximumGlueForce = 5000f )
		{
			var pos = MathConverter.Convert( entity.Position );
			var controller = new CharacterController( pos, 
					height					, 
					crouchingHeight			, 
					radius					, 
					margin					, 
					mass					,
					maximumTractionSlope	, 
					maximumSupportSlope		, 
					standingSpeed			,
					crouchingSpeed			,
					tractionForce			, 
					slidingSpeed			,
					slidingForce			,
					airSpeed				,
					airForce				, 
					jumpSpeed				, 
					slidingJumpSpeed		,
					maximumGlueForce		);
		}
	}
}
