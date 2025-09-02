using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class DiplomaArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Diploma", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.RosaDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Diploma.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Diploma", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Diploma", "description"]).Localize
		});
	}

	public int drawsReady = 3;
	public override void OnTurnStart(State state, Combat combat)
	{
		base.OnTurnStart(state, combat);
		if (drawsReady > 0)
		{
			combat.Queue(new ADrawCard(){count = 1});
			drawsReady -= 1;
		}
	}

	public override void OnCombatEnd(State state)
	{
		base.OnCombatEnd(state);
		drawsReady = 3;
	}
}
