using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Fusion;
using Fusion.Core.Mathematics;
using Fusion.Engine.Common;

namespace ShooterDemo.Core {
	public class World {

		public readonly Game Game;

		const uint MaxEntities = 1024;
		Entity[]	entities;
		uint counter	=	MaxEntities - 1;

		List<EntityView> views = new List<EntityView>();
		List<EntityController> controllers = new List<EntityController>();

		Dictionary<uint, Prefab> prefabs = new Dictionary<uint,Prefab>();


		class Prefab {
			public string Name;
			public Action Construct;
		}


		/// <summary>
		/// Gets entities.
		/// </summary>
		public Entity[] Entities {
			get { return entities; }
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="maxPlayers"></param>
		/// <param name="maxEntities"></param>
		public World ( Game game )
		{
			this.Game	=	game;
			entities	=	new Entity[ MaxEntities ];
		}



		/// <summary>
		/// Adds view.
		/// </summary>
		/// <param name="view"></param>
		void AddView( EntityView view )
		{
			views.Add( view );
		}



		/// <summary>
		/// Adds controller.
		/// </summary>
		/// <param name="controller"></param>
		void AddController ( EntityController controller )
		{
			controllers.Add( controller );
		}



		/// <summary>
		/// Gets view by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetView<T>() where T: EntityView 
		{
			return (T)views.FirstOrDefault( v => v is T );
		}



		/// <summary>
		/// Gets controller by its type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetController<T>() where T: EntityController 
		{
			return (T)controllers.FirstOrDefault( c => c is T );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefabName"></param>
		/// <param name="constructAction"></param>
		public void AddPrefab ( string prefabName, Action constructAction )
		{
			uint crc = Factory.GetPrefabID( prefabName );

			if (prefabs.ContainsKey(crc)) {
				throw new ArgumentException("Prefab '" + prefabName + "' cause hash collision with + '" + prefabs[crc].Name + "'");
			}

			prefabs.Add( crc, new Prefab(){ Name = prefabName, Construct = constructAction } );
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="gameTime"></param>
		public void Update ( GameTime gameTime, bool controllersOnly )
		{
			//
			//	Control entities :
			//
			foreach ( var controller in controllers ) {
				
				controller.Update( gameTime );
				
				var ids = controller.GetIDs();

				foreach ( var id in ids ) {
					var index = GetIndex(id);
					controller.Control( gameTime, ref entities[index] );
				}
			}

			//
			//	Present entities :
			//
			if (!controllersOnly) {
				foreach ( var view in views ) {

					view.Update( gameTime );

					var ids = view.GetIDs();

					foreach ( var id in ids ) {
						var index = GetIndex(id);
						view.Present( gameTime, ref entities[index] );
					}
				}
			}
		}



		/// <summary>
		/// Called when player enetered.
		/// </summary>
		/// <param name="guid"></param>
		public void PlayerEntered ( Guid guid )
		{
			throw new NotImplementedException("Event!");
		}



		/// <summary>
		/// Called when player left.
		/// </summary>
		/// <param name="guid"></param>
		public void PlayerLeft ( Guid guid )
		{
			throw new NotImplementedException("Event!");
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefab"></param>
		/// <param name="parent"></param>
		/// <param name="origin"></param>
		/// <param name="angles"></param>
		/// <returns></returns>
		public uint Spawn ( string prefab, uint parentId, Vector3 origin, Angles angles )
		{
			uint prefabId = Factory.GetPrefabID( prefab );


			//	search for empty entity slot :
			for (uint i=0; i<(uint)MaxEntities; i++) {

				//	increase counter
				counter++;

				//	skip zero value:
				if (counter==0) counter=1;

				uint index = counter % MaxEntities;

				if (entities[i].UniqueID==0) {

					entities[i].Initialize( prefabId, counter, parentId, origin, angles );

					return counter;
				}
			}


			Log.Warning("Can not spawn entity: {0}", prefab );

			return 0;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public uint GetIndex ( uint id )
		{
			if (id==0) {
				throw new ArgumentException("Entity ID is zero", "id");
			}

			return id % MaxEntities;
		}



		/// <summary>
		/// Check whether entity with id is dead.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool IsAlive ( uint id )
		{
			return entities[ GetIndex(id) ].UniqueID != 0;
		}



		/// <summary>
		/// Gets entity with current id.
		/// If entity is dead -> exception...
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Entity GetEntity ( uint id )
		{
			if (!IsAlive(id)) {
				throw new InvalidOperationException(string.Format("Entity #{0} is dead", id));
			}
			return entities[ GetIndex(id) ];
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public void Kill ( uint id )
		{
			if (id==0) {
				return;
			}

			entities[ GetIndex(id) ].Kill();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Write ( BinaryWriter writer )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void Read ( BinaryReader reader )
		{
		}
	}
}
