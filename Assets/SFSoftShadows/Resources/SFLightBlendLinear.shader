Shader "Hidden/SFSoftShadows/LightBlendLinear" {
	SubShader {
		Pass {
			Blend DstAlpha One, One Zero
			Cull Off
			Lighting Off
			ZTest Always
			ZWrite Off
			
			CGPROGRAM
				#pragma vertex VShader
				#pragma fragment FShader

				float _intensity;
				sampler2D _MainTex;
				
				struct VertexInput {
					float4 position : POSITION;
					float4 color : COLOR;
				};
				
				struct VertexOutput {
					float4 position : SV_POSITION;
					float4 texCoord : TEXCOORD0;
					float4 color : COLOR;
				};
				
				VertexOutput VShader(VertexInput v){
					float4 texCoord = mul(UNITY_MATRIX_P, v.position);
					VertexOutput o = {v.position, texCoord + texCoord.w, v.color};
					return o;
				}
				
				half4 FShader(VertexOutput v) : SV_Target {
					half mask = step(0.0, v.texCoord.w);
					half intensity = _intensity*tex2Dproj(_MainTex, UNITY_PROJ_COORD(v.texCoord)).a;
					return half4(v.color.rgb*mask*intensity, 1.0);
				}
			ENDCG
		}
	}
}
