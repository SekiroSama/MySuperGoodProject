Shader "GenshinToon/Face"
{
    Properties
    {
        [Header(Textures)]
        _BaseMap("Base Map", 2D) = "white" {}

        [Header(Shadow Options)]
        [Toggle (_USE_SDF_SHADOW)] _UseSDFShadow("Use SDF Shadow", Range(0,1)) = 1 // 使用SDF阴影开关
        _SDF("SDF", 2D) = "white" {} // 距离场贴图
        _ShadowMask("Shadow Mask", 2D) = "white" {} // 阴影遮罩贴图
        _ShadowColor("Shadow Color", Color) = (1, 0.87, 0.87, 1) // 阴影颜色

        [Header(Head Direction)]
        [HideInInspector] _HeadForward("Head Forward", Vector) = (0, 0, 1, 0) // 面部前方
        [HideInInspector] _HeadRight("Head Right", Vector) = (1, 0, 0, 0) // 面部右方
        [HideInInspector] _HeadUp("Head Up", Vector) = (0, 1, 0, 0) // 面部上方

        [Header(Face Blush)]
        _FaceBlushColor("Face Blush Color", Color) = (1, 0.5, 0.5, 1) // 面部腮红颜色
        _FaceBlushStrength("Face Blush Strength", Range(0,1)) = 0.5 // 面部腮红强度
    }
    SubShader
    {
       Tags
       {
            "RenderPipeline"="UniversalRenderPipeline" //指定渲染管线为URP
            "RenderType"="Opaque"
       }
       
       HLSLINCLUDE
            #pragma multi_compile _MAIN_LIGHT_SHADOWS // 主光源阴影
            #pragma multi_compile _MAIN_LIGHT_SHADOWS_CASCADE // 主光源阴影级联
            #pragma multi_compile _MAIN_LIGHT_SHADOWS_SCREEN // 主光源阴影屏幕空间

            #pragma multi_compile_fragment _LIGHT_LAYERS // 光照层
            #pragma multi_compile_fragment _LIGHT_COOKIES // 光照饼干
            #pragma multi_compile_fragment _SCREEN_SPACE_OCCLUSION // 屏幕空间遮挡
            #pragma multi_compile_fragment _ADDITIONAL_LIGHT_SHADOWS // 额外光源阴影
            #pragma multi_compile_fragment _SHADOWS_SOFT // 阴影软化
            #pragma multi_compile_fragment _REFLECTION_PROBE_BLENDING // 反射探针混合
            #pragma multi_compile_fragment _REFLECTION_PROBE_BOX_PROJECTION // 反射探针盒投影

            #pragma shader_feature_local _USE_SDF_SHADOW // Shadow开关

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" // 核心库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" // 光照库


            CBUFFER_START(UnityPerMaterial) //常量缓冲区

                //textures
                sampler2D _BaseMap; // 基础贴图

                // Shadow Options
                sampler2D _SDF; // 距离场纹理
                sampler2D _ShadowMask; // 阴影遮罩纹理
                float4 _ShadowColor; // 阴影颜色

                // Head Direction
                float3 _HeadForward; // 面部前方
                float3 _HeadRight; // 面部右方
                float3 _HeadUp; // 面部上方

                // Face Blush
                float4 _FaceBlushColor; // 面部腮红颜色
                float _FaceBlushStrength; // 面部腮红强度

            CBUFFER_END

       ENDHLSL
       
       Pass
       {
            Name "UniversalForward"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM

                #pragma vertex MainVertexShader
                #pragma fragment MainFragmentShader

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float2 uv0 : TEXCOORD0;
                    float3 normalOS : NORMAL;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv0 : TEXCOORD0;
                    float3 normalWS : NORMAL;
                };

                Varyings MainVertexShader(Attributes input)
                {
                    Varyings output;

                    // 获取顶点位置和法线
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;
                    output.uv0 = input.uv0; 
                    VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = vertexNormalInputs.normalWS;

                    return output;
                }

                half4 MainFragmentShader(Varyings input) : SV_TARGET
                {
                    Light light = GetMainLight(); // 获取主光源

                    // Normalize Vector
                    half3 N = normalize(input.normalWS);
                    half3 L = normalize(light.direction);
                    half NDotL = dot(N, L); // 法线与光线的点积
                    half3 headUpDir = normalize(_HeadUp.xyz); // 面部上方方向
                    half3 headForwardDir = normalize(_HeadForward.xyz); // 面部前方方向
                    half3 headRightDir = normalize(_HeadRight.xyz); // 面部右方方向

                    //Textures Info
                    half4 baseColor = tex2D(_BaseMap, input.uv0); 
                    half4 shadowMask = tex2D(_ShadowMask, input.uv0); // 获取阴影遮罩

                    //Lambert
                    half lambert = max(NDotL, 0); // Lambert反射模型
                    half halfLambert = lambert * 0.5 + 0.5; // 半Lambert调整
                    halfLambert *= pow (halfLambert, 2); // 增强对比度

                    //Face Shadow
                    half3 LpU = dot(L, headUpDir) / pow(length(headUpDir), 2) * headUpDir; // 计算光源方向在面部上方的投影
                    half3 LpHeadHorizon = normalize(L- LpU); // 光照方向在头部水平面上的投影
                    half value = acos(dot(LpHeadHorizon, headRightDir)) / 3.141592654; // 计算光照方向与面部右方的夹角
                    half exposeRight = step(value, 0.5); // 判断光照是来自右侧还是左侧
                    half valueR = pow(1 - value * 2, 3); // 右侧阴影强度
                    half valueL = pow(value * 2 - 1, 3); // 左侧阴影强度
                    half mixValue = lerp(valueL, valueR, exposeRight); // 混合阴影强度
                    half sdfLeft = tex2D(_SDF, half2(1 - input.uv0.x, input.uv0.y)).r; // 左侧距离场
                    half sdfRight = tex2D(_SDF, input.uv0).r; // 右侧距离场
                    half mixSdf = lerp(sdfRight, sdfLeft, exposeRight); // 采样SDF纹理
                    half sdf = step(mixValue, mixSdf); // 计算硬边界阴影
                    sdf = lerp(0, sdf, step(0, dot(LpHeadHorizon, headForwardDir))); // 计算右侧阴影
                    sdf *= shadowMask.g; // 使用G通道控制阴影强度
                    sdf = lerp(sdf, 1, shadowMask.a); // 使用A通道作为阴影遮罩

                    //Face Blush
                    half blushStrength = lerp(0, baseColor.a ,_FaceBlushStrength);// 计算腮红强度


                    //Merge Color
                    #if _USE_SDF_SHADOW
                        half3 finalColor = lerp(_ShadowColor.rgb * baseColor.rgb, baseColor.rgb, sdf); // 合并颜色
                    #else
                        half3 finalColor = baseColor.rgb * halfLambert;
                    #endif

                    finalColor = lerp(finalColor, finalColor * _FaceBlushColor.rgb , blushStrength); // 混合腮红

                    return half4(finalColor, 1); 
                }

            ENDHLSL
       }

       Pass//阴影投射Pass
       {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster" // 光照模式: 阴影投射
            }

            // 阴影投射固定设置
            ZWrite On // 开启深度写入
            ZTest LEqual // 深度测试: 小于等于
            ColorMask 0 // 不写入颜色
            Cull Off // 关闭剔除，确保双面都能投射阴影

            HLSLPROGRAM

                #pragma multi_compile_instancing // 启用GPU实例化编译
                #pragma multi_compile _ DOTS_INSTANCING_ON // 启用DOTS实例化编译
                #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW // 启用点光源阴影

                #pragma vertex ShadowVS
                #pragma fragment ShadowFS

                //编译器会自动为我们提供以下内置变量：
                float3 _LightDirection; // 光源方向
                float3 _LightPosition; // 光源位置

                struct Attributes
                {
                    float4 positionOS : POSITION; // 顶点位置
                    float3 normalOS : NORMAL; // 顶点法线
                };
                
                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                };

                // 将阴影的世界空间顶点位置转换为适合阴影投射的裁剪空间位置
                float4 GetShadowPositionHClip(Attributes input)
                {
                    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz); // 将本地空间顶点坐标转换为世界空间顶点坐标
                    float3 normalWS = TransformObjectToWorldNormal(input.normalOS); // 将本地空间法线转换为世界空间法线

                    #if _CASTING_PUNCTUAL_LIGHT_SHADOW // 点光源
                        float3 lightDirectionWS = normalize(_LightPosition - positionWS); // 计算光源方向
                    #else // 平行光
                        float3 lightDirectionWS = _LightDirection; // 使用预定义的光源方向
                    #endif

                    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS)); // 应用阴影偏移

                    // 根据平台的Z缓冲区方向调整Z值
                    #if UNITY_REVERSED_Z // 反转Z缓冲区
                        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE); // 限制Z值在近裁剪平面以下
                    #else // 正向Z缓冲区
                        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE); // 限制Z值在远裁剪平面以上
                    #endif

                    return positionCS; // 返回裁剪空间顶点坐标
                }

                //顶点着色器
                Varyings ShadowVS(Attributes input)
                {
                    Varyings output;
                    output.positionCS = GetShadowPositionHClip(input); // 获取阴影位置
                    return output;
                }

                //片元着色器
                half4 ShadowFS(Varyings input) : SV_TARGET
                {
                    return half4(0, 0, 0, 1); // 输出纯黑色，表示完全遮挡
                }

            ENDHLSL
       }
    }
}
