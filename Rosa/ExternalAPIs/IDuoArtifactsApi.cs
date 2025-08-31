using System;
using System.Collections.Generic;

namespace Flipbop.Rosa;

public interface IDuoArtifactsApi
{
	Deck DuoArtifactVanillaDeck { get; }

	void RegisterDuoArtifact(Type type, IEnumerable<Deck> combo);
	void RegisterDuoArtifact<TArtifact>(IEnumerable<Deck> combo) where TArtifact : Artifact;
}