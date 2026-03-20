Shader "GenshinToon/Body"
{
    Properties
    {
        [Header(Textures)]
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1) // 基础颜色
        _LightMap("Light Map", 2D) = "white" {}// 光照贴图
        [Toggle(_USE_lIGHTMAP_AO)] _USE_lIGHTMAP_AO ("USE lIGHTMAP AO", Range(0,1)) = 1 //AO开关

        [Header(Ramp Shadow)]
        _RampTex("Ramp Tex", 2D) = "white" {} // Ramp贴图
        [Toggle(_USE_RAMP_SHADOW)] _USE_RAMP_SHADOW ("USE RAMP Shadow", Range(0,1)) = 1 //色阶阴影开关
        _ShadowRampWidth("Shadow Ramp width", Float) = 1 // 阴影边缘宽度
        _ShadowPosition("Shadow Position", Float) = 0.55 // 阴影位置
        _ShadowSoftness("Shadow Softness", Range(0, 10)) = 0.5 // 阴影柔和度
        [Toggle] _USERAMPSHADOW2 ("USE RAMP Shadow 2", Range(0,1)) = 1 // 使用第2行的RAMP阴影开关
        [Toggle] _USERAMPSHADOW3 ("USE RAMP Shadow 3", Range(0,1)) = 1 // 使用第3行的RAMP阴影开关
        [Toggle] _USERAMPSHADOW4 ("USE RAMP Shadow 4", Range(0,1)) = 1 // 使用第4行的RAMP阴影开关
        [Toggle] _USERAMPSHADOW5 ("USE RAMP Shadow 5", Range(0,1)) = 1 // 使用第5行的RAMP阴影开关

        [Header(Lighting Options)]
        _DayOrNight ("Day Or Night", Range(0,1)) = 0//日夜切换参数

        [Header(Outline)]
        _OutLineColor("Outline Color", Color) = (0, 0, 0, 1) // 轮廓线颜色
        _OutLineWidth("Outline Width", Range(0, 0.001)) = 0.001 // 轮廓线宽度
        _OutLineMaxDist("Outline Max Distance", Range(0.1, 20)) = 5 // 轮廓线最大距离
        _OutLineDistPower("Outline DistPower", Range(0.1, 10)) = 0.5 // 轮廓线强度系数
        
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

                #pragma shader_feature_local _USE_lIGHTMAP_AO // AO开关
                #pragma shader_feature_local _USE_RAMP_SHADOW // Shadow开关

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" // 核心库
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl" // 光照库

                CBUFFER_START(UnityPerMaterial) //常量缓冲区

                    //Textures
                    sampler2D _BaseMap; // 基础贴图
                    float4 _BaseColor; // 基础颜色
                    sampler2D _LightMap; // 光照贴图

                    //Ramp Shadow
                    sampler2D _RampTex; // Ramp贴图
                    float _ShadowRampWidth; // 阴影边缘宽度
                    float _ShadowPosition; // 阴影位置
                    float _ShadowSoftness; // 阴影柔和度
                    float _USERAMPSHADOW2; // 使用第2行的RAMP阴影开关
                    float _USERAMPSHADOW3; // 使用第3行的RAMP阴影开关
                    float _USERAMPSHADOW4; // 使用第4行的RAMP阴影开关
                    float _USERAMPSHADOW5; // 使用第5行的RAMP阴影开关

                    //Lighting Options
                    float _DayOrNight; //日夜切换参数

                    //Outline
                    float4 _OutLineColor; // 轮廓线颜色
                    float _OutLineWidth; // 轮廓线宽度
                    float _OutLineMaxDist; // 轮廓线最大距离
                    float _OutLineDistPower; // 轮廓线强度系数
                    
                CBUFFER_END //常量缓冲区结束

                // 官方版本的RampShadowID函数
                float RampShadowID(float input, float useShadow2, float useShadow3, float useShadow4, float useShadow5, 
                    float shadowValue1, float shadowValue2, float shadowValue3, float shadowValue4, float shadowValue5)
                {
                    // 根据input值将模型分为5个区域
                    float v1 = step(0.6, input) * step(input, 0.8); // 0.6-0.8区域
                    float v2 = step(0.4, input) * step(input, 0.6); // 0.4-0.6区域
                    float v3 = step(0.2, input) * step(input, 0.4); // 0.2-0.4区域
                    float v4 = step(input, 0.2);                    // 0-0.2区域

                    // 根据开关控制是否使用不同材质的值
                    float blend12 = lerp(shadowValue1, shadowValue2, useShadow2);
                    float blend15 = lerp(shadowValue1, shadowValue5, useShadow5);
                    float blend13 = lerp(shadowValue1, shadowValue3, useShadow3);
                    float blend14 = lerp(shadowValue1, shadowValue4, useShadow4);

                    // 根据区域选择对应的材质值
                    float result = blend12;                // 默认使用材质1或2
                    result = lerp(result, blend15, v1);    // 0.6-0.8区域使用材质5
                    result = lerp(result, blend13, v2);    // 0.4-0.6区域使用材质3
                    result = lerp(result, blend14, v3);    // 0.2-0.4区域使用材质4
                    result = lerp(result, shadowValue1, v4); // 0-0.2区域使用材质1

                    return result;
                }

        ENDHLSL // 公共代码块结束
       
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
                    float4 color : COLOR0; // 顶点颜色
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    float4 color : TEXCOORD2; // 顶点颜色
                };

                Varyings MainVertexShader(Attributes input)
                {
                    Varyings output;

                    // 获取顶点位置和法线
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;
                    output.uv = input.uv0; 
                    VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = vertexNormalInputs.normalWS;

                    //color
                    output.color = input.color; // 将顶点颜色传递到片段着色器

                    return output;
                }

                half4 MainFragmentShader(Varyings input) : SV_TARGET
                {
                    Light light = GetMainLight(); // 获取主光源
                    half4 vertexColor = input.color; // 获取顶点颜色

                    //Normalize Vector
                    half3 N = normalize(input.normalWS);
                    half3 L = normalize(light.direction);
                    half NDotL = dot(N, L); // 法线与光线的点积

                    //Textures Info
                    half4 baseMap = tex2D(_BaseMap, input.uv); //纹理采样
                    half4 lightMap = tex2D(_LightMap, input.uv); //光照采样

                    //Lambert
                    half lambert = max(NDotL, 0); // Lambert反射模型
                    half halflambert = lambert * 0.5 + 0.5; // 半Lambert调整
                    halflambert *= pow (halflambert, 2); // 增强对比度
                    half lambertstep = smoothstep(0.01, 0.4, halflambert); //在[0.01, 0.4]范围内平滑过渡
                    half shadowFactor = lerp(0, halflambert, lambertstep); //计算阴影因子

                    //AO
                    #if _USE_lIGHTMAP_AO
                        half ambient = lightMap.g;// 环境光
                    #else
                        half ambient = halflambert; // 没有AO时环境光为1
                    #endif
                    half shadow = (ambient + halflambert) * 0.5; // 环境光遮蔽 shadow值越小阴影越重
                    // shadow = 0.95 <= ambient? 1: shadow;
                    // shadow = ambient <= 0.05? 0: shadow;
                    shadow = lerp(shadow, 1, step(0.95, ambient)); // 亮度大于0.95时增强亮度
                    shadow = lerp(shadow, 0, step(ambient, 0.05)); // 亮度小于0.05时增强阴影
                    
                    half isShadowArea = step(shadow, _ShadowPosition);//是否处于阴影区域,当shadow小于等于_ShadowPosition说明处于阴影区域
                    half shadowDepth = saturate(_ShadowPosition - shadow) / _ShadowRampWidth; // 计算阴影深度
                    shadowDepth = pow(shadowDepth, _ShadowSoftness); // 根据柔和度调整阴影深度
                    shadowDepth = min(shadowDepth, 1);//限制阴影深度最大值为1
                    half rampWidthFactor = vertexColor.g * 2 * _ShadowRampWidth;//根据顶点颜色G通道调整阴影边缘宽度
                    // half shadowPosition = (_ShadowPosition - shadowFactor) / _ShadowPosition;//带入阴影因子计算阴影位置

                    //Ramp
                    half rampU = 1 - saturate(shadowDepth / rampWidthFactor); // 计算Ramp采样的横坐标
                    half rampID = RampShadowID(lightMap.a, _USERAMPSHADOW2, _USERAMPSHADOW3, _USERAMPSHADOW4, _USERAMPSHADOW5, 1, 2, 3, 4, 5); // 根据光照贴图的Alpha通道选择Ramp采样的ID
                    half rampV = 0.45 - (rampID - 1) * 0.1; // 根据光照贴图的Alpha通道选择Ramp采样的纵坐标
                    half2 rampDayUV = half2(rampU, rampV + 0.5);// 计算Ramp白天采样坐标
                    half3 rampDayColor = tex2D(_RampTex, rampDayUV).rgb; // 从Ramp贴图采样颜色
                    half2 rampNightUV = half2(rampU, rampV);// 计算Ramp夜晚采样坐标
                    half3 rampNightColor = tex2D(_RampTex, rampNightUV).rgb; // 从Ramp贴图采样颜色
                    half3 rampColor = lerp(rampNightColor, rampDayColor, _DayOrNight); // 根据日夜参数混合Ramp颜色

                    //Merge Color
                    #if _USE_RAMP_SHADOW
                        half3 finalColor = baseMap.rgb * _BaseColor.rgb * rampColor * (isShadowArea ? 1 : 1.2);//采用Ramp阴影时
                    #else
                        half3 finalColor = baseMap.rgb * _BaseColor.rgb * halflambert * (shadow + 0.2);//采用lambert阴影
                    #endif

                    return half4(finalColor.rgb, 1); 
                    // return half4(vertexColor.ggg, 1);
                }

            ENDHLSL
        }

        Pass
        {
            Name "OutLine"
            Tags
            {
                "LightMode" = "SRPDefaultUnlit" // 使用SRP默认的非光照模式，确保轮廓线不受光照影响
                "Queue" = "Geometry+1" // 将轮廓线渲染在几何体之后，确保轮廓线覆盖在模型上
            }

            ZWrite On // 开启深度写入，确保轮廓线正确遮挡
            ZTest LEqual // 深度测试: 小于等于，确保轮廓线正确遮挡
            Cull Front // 剔除前面，渲染背面以实现轮

            HLSLPROGRAM

                #pragma vertex OutLineVertexShader
                #pragma fragment OutLineFragmentShader

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 normalWS : TEXCOORD0;
                };

                Varyings OutLineVertexShader(Attributes input)
                {
                    Varyings output;
                    
                    // 获取顶点位置和法线
                    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz); // 将本地空间顶点坐标转换为世界空间顶点坐标
                    float dist = distance(positionWS, GetCameraPositionWS()); 
                    float outLineStrength = _OutLineDistPower * min(dist, _OutLineMaxDist); // 根据距离计算轮廓线强度

                    input.positionOS.xyz = input.positionOS.xyz + float4(input.normalOS * _OutLineWidth * outLineStrength, 0); // 根据法线方向和轮廓线宽度偏移顶点位置

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                    output.positionCS = vertexInput.positionCS;
                    VertexNormalInputs vertexNormalInputs = GetVertexNormalInputs(input.normalOS);
                    output.normalWS = vertexNormalInputs.normalWS;

                    return output;
                }

                half4 OutLineFragmentShader(Varyings input) : SV_TARGET
                {
                    return _OutLineColor; // 输出轮廓线颜色
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
