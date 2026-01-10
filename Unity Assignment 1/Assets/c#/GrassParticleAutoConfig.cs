using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrassParticleAutoConfig : MonoBehaviour
{
    [ContextMenu("Pei Zhi Sui Cao")]
    public void ConfigureAsGrass()
    {
        ParticleSystem lizi = GetComponent<ParticleSystem>();

        ParticleSystem.MainModule m = lizi.main;
        ParticleSystem.EmissionModule e = lizi.emission;
        ParticleSystem.ShapeModule s = lizi.shape;
        ParticleSystem.RotationOverLifetimeModule r = lizi.rotationOverLifetime;
        ParticleSystem.SizeOverLifetimeModule daxiao = lizi.sizeOverLifetime;

        ParticleSystemRenderer rend = GetComponent<ParticleSystemRenderer>();

        m.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        m.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        m.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        m.gravityModifier = 1.8f;
        m.simulationSpace = ParticleSystemSimulationSpace.World;
        m.stopAction = ParticleSystemStopAction.Destroy;

        e.rateOverTime = 0;
        ParticleSystem.Burst[] b = new ParticleSystem.Burst[1];
        b[0] = new ParticleSystem.Burst(0, 20);
        e.SetBursts(b);

        s.shapeType = ParticleSystemShapeType.Cone;
        s.angle = 30f;
        s.radius = 0.15f;

        r.enabled = true;
        r.z = new ParticleSystem.MinMaxCurve(-720f, 720f);

        daxiao.enabled = true;
        AnimationCurve quxian = new AnimationCurve();
        quxian.AddKey(0f, 1f);
        quxian.AddKey(0.7f, 0.8f);
        quxian.AddKey(1f, 0f);
        daxiao.size = new ParticleSystem.MinMaxCurve(1f, quxian);

        if (rend != null)
        {
            rend.renderMode = ParticleSystemRenderMode.Mesh;

            rend.lengthScale = 2f;
            rend.velocityScale = 0.1f;

            if (rend.sharedMaterial == null || rend.sharedMaterial.name.Contains("Default"))
            {
                Shader s1 = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (s1 == null)
                {
                    s1 = Shader.Find("Particles/Standard Unlit");
                }

                if (s1 != null)
                {
                    Material m1 = new Material(s1);
                    m1.color = new Color(0.2f, 0.6f, 0.1f);
                    if (m1.HasProperty("_BaseColor"))
                    {
                        m1.SetColor("_BaseColor", m1.color);
                    }
                    rend.sharedMaterial = m1;
                }
            }
        }

        Debug.Log("Sui Cao Ok!");
    }
}