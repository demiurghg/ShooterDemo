using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.EntityStateManagement;
using BEPUphysics.PositionUpdating;

namespace ShooterDemo.Controllers {

	public class Projectiles : EntityController<Projectiles.Projectile> {

		Random rand = new Random();

		public class Projectile {
			public float Velocity;
			public float Impulse;
			public short Damage;
			public float LifeTime;
			public string ExplosionFX;
			public float Radius;
		}


		readonly Space space;
		MPWorld world;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		/// <param name="space"></param>
		public Projectiles ( World world, Space space ) : base(world)
		{
			this.world	=	(MPWorld)world;
			this.space	=	space;
		}


		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, bool dirty )
		{
			IterateObjects( dirty, (d,e,proj) => {
				
				UpdateProjectile( e, proj, elapsedTime );

			});
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="projEntity"></param>
		/// <param name="projectile"></param>
		public void UpdateProjectile ( Entity projEntity, Projectile projectile, float elapsedTime )
		{
			var origin	=	projEntity.Position;
			var dir		=	Matrix.RotationQuaternion( projEntity.Rotation ).Forward;
			var target	=	origin + dir * projectile.Velocity * elapsedTime;

			projectile.LifeTime -= elapsedTime;

			Vector3 hitNormal, hitPoint;
			Entity  hitEntity;

			var parent	=	world.GetEntity( projEntity.ParentID );


			if ( projectile.LifeTime <= 0 ) {
				world.Kill( projEntity.ID );
			}

			if ( world.RayCastAgainstAll( origin, target, out hitNormal, out hitPoint, out hitEntity, parent ) ) {
				
				if (hitEntity!=null) {
					hitEntity.Kick( dir * projectile.Impulse, hitPoint );
				}

				if (projectile.Radius>0) {
					
					var list = world.WeaponOverlap( hitPoint, projectile.Radius, hitEntity );

					foreach ( var e in list ) {
						var delta = e.Position - hitPoint;
						var dist  = delta.Length() + 0.00001f;
						var ndir  = delta / dist;
						var imp   = Math.Max(0, (projectile.Radius - dist) / projectile.Radius * projectile.Impulse);

						e.Kick( ndir * imp, e.Position + rand.UniformRadialDistribution(0.1f, 0.1f) );
					}
				}


				world.SpawnFX( projectile.ExplosionFX, projEntity.ParentID, hitPoint, hitNormal );
				projEntity.Move( hitPoint, projEntity.Rotation, dir * projectile.Velocity );

				world.Kill( projEntity.ID );

			} else {
				projEntity.Move( target, projEntity.Rotation, dir.Normalized() * projectile.Velocity );
			}
		}




		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{
			Projectile obj;
			
			if ( RemoveObject( id, out obj ) ) {
			}
		}



		/// <summary>
		/// 
		/// </summary>
		public void AddProjectile ( Entity entity, string explosionFX, float velocity, float radius, short damage, float impulse, float lifeTime )
		{
			var proj = new Projectile {	
				ExplosionFX = explosionFX, 
				Damage		= damage, 
				Velocity	= velocity, 
				Impulse		= impulse, 
				LifeTime	= lifeTime,
				Radius		= radius,
			};

			AddObject( entity.ID, proj ); 
		}
	}
}
