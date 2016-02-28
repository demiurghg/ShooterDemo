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


namespace ShooterDemo.Views {
	public class ModelView : EntityView<ModelView.Model> {

		
		public class Model {
			readonly public string ScenePath;
			readonly public string NodeName;
			readonly public Matrix PreTransform;
			readonly public Matrix PostTransform;
			public MeshInstance Instance;

			public Model ( string scenePath, string nodeName, Matrix preTransform, Matrix postTransform )
			{
				 ScenePath		=	scenePath;
				 NodeName		=	nodeName;
				 PreTransform	=	preTransform;
				 PostTransform	=	postTransform;
			}


			public void Reload ( RenderSystem rs, ContentManager content )
			{
				if (Instance!=null) {
					if (!rs.RenderWorld.Instances.Remove( Instance )) {
						Log.Warning("Failed to remove {0}|{1}", ScenePath, NodeName );
					}
				}

				var scene = content.Load<Scene>( ScenePath, (Scene)null );

				if (scene==null) {
					return;
				}

				var node  = scene.Nodes.FirstOrDefault( n => n.Name == NodeName );

				if (node==null) {
					Log.Warning("Scene '{0}' does not contain node '{1}'", ScenePath, NodeName );
					return;
				}

				if (node.MeshIndex<0) {
					Log.Warning("Node '{0}|{1}' does not contain mesh", ScenePath, NodeName );
					return;
				}

				var mesh		=	scene.Meshes[node.MeshIndex];

				var defMtrl		=	rs.DefaultMaterial;
				var materials	=	scene.Materials.Select( m => content.Load<MaterialInstance>( m.Name, defMtrl ) ).ToArray();

				Instance		= new MeshInstance( rs, scene, mesh, materials );

				rs.RenderWorld.Instances.Add( Instance );
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="game"></param>
		public ModelView ( World world ) : base(world)
		{
			Game.Reloading += Game_Reloading;
		}



		void Game_Reloading ( object sender, EventArgs e )
		{
			IterateObjects( (ent,m) => m.Reload(Game.RenderSystem, World.Content) );
		}





		/// <summary>
		/// Hot reload?
		/// </summary>
		/// <param name="scenePath"></param>
		/// <param name="nodeName"></param>
		/// <param name="preTransform"></param>
		public void AddModel ( Entity entity, string scenePath, string nodeName, Matrix preTransform, Matrix postTransform )
		{
			var model = new Model( scenePath, nodeName, preTransform, postTransform ); 

			model.Reload( Game.RenderSystem, World.Content );

			AddObject( entity.ID, model ); 
		} 



		/// <summary>
		/// Updates visible meshes
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update ( float elapsedTime, float lerpFactor )
		{
			IterateObjects( (e,m) => {
				m.Instance.World	=	m.PreTransform * e.GetWorldMatrix(lerpFactor) * m.PostTransform;
				m.Instance.Visible	=	e.UserGuid != World.GameClient.Guid;
			});
		}



		/// <summary>
		/// Removes entity
		/// </summary>
		/// <param name="id"></param>
		public override void Kill ( uint id )
		{	
			Model model;

			if ( RemoveObject( id, out model ) ) {
				Game.RenderSystem.RenderWorld.Instances.Remove( model.Instance );
			}
		}

	}
}
