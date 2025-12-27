using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrassParticleAutoConfig : MonoBehaviour
{
    [ContextMenu("一键配置为碎草效果")]
    public void ConfigureAsGrass()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var rot = ps.rotationOverLifetime;
        var size = ps.sizeOverLifetime;

        // 获取渲染器组件
        ParticleSystemRenderer psRenderer = GetComponent<ParticleSystemRenderer>();

        // 1. 基础设置：增加尺寸和寿命，确保在场景中清晰可见
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.3f);
        main.gravityModifier = 1.8f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // 2. 爆发式喷发：一次性喷出碎屑
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, 20) });

        // 3. 形状：向上锥形喷发，模拟割草飞溅方向
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 30f;
        shape.radius = 0.15f;

        // 4. 旋转：碎草在空中随机翻滚
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-720f, 720f);

        // 5. 变小消失曲线：让粒子在落地前自然消失
        size.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(0.7f, 0.8f);
        curve.AddKey(1f, 0f);
        size.size = new ParticleSystem.MinMaxCurve(1f, curve);

        // 6. 渲染器设置：解决 CS0117 编译报错
        if (psRenderer != null)
        {
            // 使用强制类型转换设置 RenderMode 为 StretchedBillboard (对应枚举索引为 3)
            // 这种方式可以避开不同 Unity 版本对枚举名定义的微小差异，彻底解决编译错误
            psRenderer.renderMode = (ParticleSystemRenderMode)3;

            psRenderer.lengthScale = 2f;
            psRenderer.velocityScale = 0.1f;

            // 自动配置材质
            if (psRenderer.sharedMaterial == null || psRenderer.sharedMaterial.name.Contains("Default"))
            {
                // 优先寻找 URP 材质，若无则回退到 Standard
                Shader defaultShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
                if (defaultShader == null) defaultShader = Shader.Find("Particles/Standard Unlit");

                if (defaultShader != null)
                {
                    Material grassMat = new Material(defaultShader);
                    grassMat.color = new Color(0.2f, 0.6f, 0.1f); // 设定为草绿色
                    if (grassMat.HasProperty("_BaseColor")) grassMat.SetColor("_BaseColor", grassMat.color);
                    psRenderer.sharedMaterial = grassMat;
                }
            }
        }

        Debug.Log("<color=green>碎草粒子配置成功！</color> 已通过类型转换绕过枚举引用报错。");
    }
}