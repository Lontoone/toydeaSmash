Shader "Unlit/SpriteMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
	}
		SubShader
	{
		Tags {
			"RenderType" = "Transparent"
			"RenderQueue" = "Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};


			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Mask;
			float4 _Mask_ST;


			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{

				fixed4 main_col = tex2D(_MainTex, i.uv);
				fixed4 mask_color = tex2D(_Mask,i.uv);

				//fixed4 res= main_col* (1 - mask_color.z) + mask_color * mask_color.z * i.color; 
				//fixed4 res = main_col * (1 - mask_color.a) + mask_color * mask_color.a * i.color;
				fixed _a=(1 - mask_color.a);
				if(_a>0){
					fixed4 res = main_col;
					return res;}
				else{
					fixed4 res = main_col  * i.color;
					return res;
				}

			}
			ENDCG
		}
	}
}