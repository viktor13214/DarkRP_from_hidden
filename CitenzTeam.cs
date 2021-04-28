using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace HiddenGamemode
{
	class CitenzTeam : BaseTeam
	{
		public override string HudClassName => "team_citenz";
		public override string Name => "Citenz";

		private Battery _batteryHud;
		private Radar _radarHud;

		public override void SupplyLoadout(Player player)
		{
			player.ClearAmmo();
			player.Inventory.DeleteContents();
			player.Inventory.Add();

			if (player.Deployment == DeploymentType.CITENZ_CITY)
			{
				player.Inventory.Add();
				player.GiveAmmo();
			}
			else
			{
				player.Inventory.Add();
				player.GiveAmmo();
			}
		}

		public override void OnStart(Player player)
		{
			player.ClearAmmo();
			player.Inventory.DeleteContents();

			player.SetModel("models/citizen/citizen.vmdl");

			if (Host.IsServer)
			{
				player.RemoveClothing();
				player.AttachClothing("models/citizen_clothes/trousers/trousers.lab.vmdl");
				player.AttachClothing("models/citizen_clothes/jacket/labcoat.vmdl");
				player.AttachClothing("models/citizen_clothes/shoes/shoes.workboots.vmdl");
				player.AttachClothing("models/citizen_clothes/hat/hat_securityhelmet.vmdl");
			}

			player.EnableAllCollisions = true;
			player.EnableDrawing = true;
			player.EnableHideInFirstPerson = true;
			player.EnableShadowInFirstPerson = true;

			player.Controller = new IrisController();
			player.Camera = new FirstPersonCamera();
		}

		public override void OnJoin(Player player)
		{
			Log.Info($"{player.Name} joined the Military team.");

			if (Host.IsClient && player.IsLocalPlayer)
			{
				_radarHud = Sandbox.Hud.CurrentPanel.AddChild<Radar>();

				// TODO: Let's try not having a battery HUD. Does it make it spookier?
				//_batteryHud = Sandbox.Hud.CurrentPanel.AddChild<Battery>();
			}

			base.OnJoin(player);
		}

		public override void AddDeployments(Deployment panel, Action<DeploymentType> callback)
		{
			panel.AddDeployment(new DeploymentInfo
			{
				Title = "Legally Compliant",
				Description = "you are a legally obedient citizen trusted by the police",
				ClassName = "Legally",
				OnDeploy = () => callback(DeploymentType.Legally_Compliant)
			});

			panel.AddDeployment(new DeploymentInfo
			{
				Title = "Criminal",
				Description = "you are registered with the police they don't trust you.",
				ClassName = "Criminal",
				OnDeploy = () => callback(DeploymentType.Criminal)
			});
		}

		public override void OnPlayerKilled(Player player)
		{
			player.GlowActive = false;
		}

		public override void OnLeave(Player player)
		{
			Log.Info($"{player.Name} left the Military team.");

			if (player.IsLocalPlayer)
			{
				if (_radarHud != null)
				{
					_radarHud.Delete(true);
					_radarHud = null;
				}

				if (_batteryHud != null)
				{
					_batteryHud.Delete(true);
					_batteryHud = null;
				}
			}

			base.OnLeave(player);
		}
	}
}
