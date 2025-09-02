using Nanoray.PluginManager;
using Nickel;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class EgoArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Ego", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Ego.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Ego", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Ego", "description"]).Localize
		});
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		base.OnTurnEnd(state, combat);
		int statusCount = 0;
		for (int i = 0; i >= combat.otherShip.statusEffects.Count; i++)
		{
			//if ()
		}

	}
}
