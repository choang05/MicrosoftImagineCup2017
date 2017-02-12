Shader "Hidden/SFSoftShadows/HDRClamp" {
	SubShader {
		Pass {
			BlendOp Max
			Blend One One
			Cull Off
			Lighting Off
			ZTest Always
			ZWrite Off
			
			CGPROGRAM
				#pragma target 3.0
				#pragma vertex VShader
				#pragma fragment FShader

				struct VertexInput {
					float3 position : POSITION;
				};
				
				float4 VShader(VertexInput IN) : SV_POSITION {
					return float4(IN.position, 1.0);
				}
				
				half4 FShader(void) : SV_Target {
					return half4(0.0, 0.0, 0.0, 0.0);
				}
			ENDCG
		}
	}
}
