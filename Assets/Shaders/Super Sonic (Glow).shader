// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/

Shader "Shader Forge/Super Sonic (Glow)" {
    Properties {
        [PerRendererData]_MainTex ("MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _Stencil ("Stencil ID", Float) = 0
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilOpFail ("Stencil Fail Operation", Float) = 0
        _StencilOpZFail ("Stencil Z-Fail Operation", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            Stencil {
                Ref [_Stencil]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
                Comp [_StencilComp]
                Pass [_StencilOp]
                Fail [_StencilOpFail]
                ZFail [_StencilOpZFail]
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Color;
            float3 Function_node_Lerp( float3 In , float3 From , float3 To , float Range , float Fuzziness )
            {
                float Distance = distance(From, In);
                return lerp(To, In, saturate((Distance - Range) / Fuzziness));
            }
            
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float node_603 = (_MainTex_var.a*_Color.a*i.vertexColor.a); // A
                float3 emissive =
                Function_node_Lerp(
                    Function_node_Lerp(
                        Function_node_Lerp(
                            Function_node_Lerp(
                                Function_node_Lerp(
                                    Function_node_Lerp(
                                        Function_node_Lerp(
                                            Function_node_Lerp(
                                                Function_node_Lerp(
                                                    ((_MainTex_var.rgb*_Color.rgb*i.vertexColor.rgb)*node_603) ,

                                                    float3(0.0*node_603, 0.0*node_603, 0.50196078*node_603) ,
                                                    float3(0.9411765*node_603, 0.8470588*node_603, 0.596078431*node_603) , 0.0 , 0.01 ) ,

                                                    float3(0.0*node_603, 0.2196078*node_603, 0.7529411*node_603) ,
                                                    float3(0.9411765*node_603, 0.8784313*node_603, 0.6901960*node_603) , 0.0 , 0.01 ) ,

                                                    float3(0.0*node_603, 0.4078431*node_603, 0.9411765*node_603) ,
                                                    float3(0.9411765*node_603, 0.9098039*node_603, 0.7529411*node_603) , 0.0 , 0.01 ) ,

                                                    float3(0.09411764*node_603, 0.5333333*node_603, 0.9411765*node_603) ,
                                                    float3(0.9411765*node_603, 0.9411765*node_603, 0.8470588*node_603) , 0.0 , 0.01 ),

                                                    float3(0.18823529*node_603, 0.6274509*node_603, 0.9411765*node_603) ,
                                                    float3(0.9411765*node_603, 0.9411765*node_603, 0.9411765*node_603) , 0.0 , 0.01 ),

                                                    float3(0.40784313*node_603, 0.8156862*node_603, 0.9411765*node_603) ,
                                                    float3(0.9411765*node_603, 0.9411765*node_603, 0.97254901*node_603) , 0.0 , 0.01 ),

                                                    float3(0.0941176*node_603, 0.3450980*node_603, 0.4078431*node_603) ,
                                                    float3(0.1450980*node_603, 0.3137254*node_603, 0.3568627*node_603) , 0.0 , 0.01 ),

                                                    float3(0.3764705*node_603, 0.6274509*node_603, 0.6901960*node_603) ,
                                                    float3(0.4274509*node_603, 0.5960784*node_603, 0.6392156*node_603) , 0.0 , 0.01 ),

                                                    float3(0.6274509*node_603, 0.8784313*node_603, 0.8784313*node_603) ,
                                                    float3(0.6705882*node_603, 0.8352941*node_603, 0.8352941*node_603) , 0.0 , 0.01 );
                return fixed4(emissive, node_603);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 xboxone ps4 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
