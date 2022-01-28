Shader "ShadowReceiver"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 projectorSpacePos : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
            };
            
            sampler2D _ShadowProjectorTexture;
            float4x4 _ShadowProjectorMatrixVP;
            float4 _ShadowProjectorPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.projectorSpacePos = mul(mul(_ShadowProjectorMatrixVP, unity_ObjectToWorld), v.vertex);
                o.projectorSpacePos = ComputeScreenPos(o.projectorSpacePos);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.projectorSpacePos.xyz /= i.projectorSpacePos.w;
                float2 uv = i.projectorSpacePos.xy;
                float4 projectorTex = tex2D(_ShadowProjectorTexture, uv);
                // カメラの範囲外には適用しない
                fixed3 isOut = step((i.projectorSpacePos - 0.5) * sign(i.projectorSpacePos), 0.5);
                float alpha = isOut.x * isOut.y * isOut.z;
                // プロジェクターから見て裏側の面には適用しない
                alpha *= step(-dot(lerp(-_ShadowProjectorPos.xyz, _ShadowProjectorPos.xyz - i.worldPos, _ShadowProjectorPos.w), i.worldNormal), 0);
                float4 col = lerp(1, projectorTex, alpha);
                float gray = dot(col.rgb, fixed3(0.299, 0.587, 0.114));
                if(gray<0.5){
	                return fixed4(0, 0, 0, 1);
                }
                else{
	                return fixed4(1, 1, 1, 1);
                }
            }
            ENDCG
        }
    }
}