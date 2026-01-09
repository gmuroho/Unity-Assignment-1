using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrassParticleAutoConfig : MonoBehaviour
{
    [ContextMenu("配置为碎草")]
    public void ConfigureAsGrass()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var rot = ps.rotationOverLifetime;
        var size = ps.sizeOverLifetime;

        
        ParticleSystemRenderer psRenderer = GetComponent<ParticleSystemRenderer>();

        
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.gravityModifier = 1.8f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 20) });

        
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.15f;

        
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-720f, 720f);

        
        size.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.7f, 0.8f);
        curve.AddKey(1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);

        
        if (psRenderer != null)
        {
            
            
            psRenderer.renderMode = (ParticleSystemRenderMode)3;

            psRenderer.lengthScale = 2f;
            psRenderer.velocityScale = 0.1f;

            
            if (psRenderer.sharedMaterial == null || psRenderer.sharedMaterial.name.Contains("Default"))
            {
                
                Shader defaultShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (defaultShader == null) defaultShader = Shader.Find("Particles/Standard Unlit");

                if (defaultShader != null)
                {
                    Material grassMat = new Material(defaultShader);
                    grassMat.color = new Color(0.2f, 0.6f, 0.1f); 
                    if (grassMat.HasProperty("_BaseColor")) grassMat.SetColor("_BaseColor", grassMat.color);
                    psRenderer.sharedMaterial = grassMat;
                }
            }
        }

        Debug.Log("<color=green>碎草粒子配置成功！</color> 已通过类型转换绕过枚举引用报错?");
    }
}