using HarmonyLib;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class RetainerArtifact : Artifact, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Retainer", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = ModEntry.Instance.CleoDeck.Deck,
				pools = ModEntry.GetArtifactPools(MethodBase.GetCurrentMethod()!.DeclaringType!)
			},
			Sprite = helper.Content.Sprites.RegisterSprite(ModEntry.Instance.Package.PackageRoot.GetRelativeFile("assets/Artifacts/Retainer.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Retainer", "name"]).Localize,
			Description = ModEntry.Instance.AnyLocalizations.Bind(["artifact", "Retainer", "description"]).Localize
		});
	}
	public override List<Tooltip>? GetExtraTooltips()
		=> [new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve A")
		{
			Icon = ModEntry.Instance.ImproveAIcon.Sprite,
			TitleColor = Colors.action,
			Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveA", "description"])
		}, new GlossaryTooltip($"action.{ModEntry.Instance.Package.Manifest.UniqueName}::Improve B")
		{
			Icon = ModEntry.Instance.ImproveBIcon.Sprite,
			TitleColor = Colors.action,
			Title = ModEntry.Instance.Localizations.Localize(["action", "ImproveB", "name"]),
			Description = ModEntry.Instance.Localizations.Localize(["action", "ImproveB", "description"])
		}];
}
