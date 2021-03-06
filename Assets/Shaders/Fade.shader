﻿Shader "Custom/Fade" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
	}

		SubShader{
		Tags{ "Queue" = "Overlay-1" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Cull Front
		ZTest Always
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"

		struct appdata_t {
		float4 vertex : POSITION;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
	};

	fixed4 _Color;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 col = _Color;
	return col;
	}
		ENDCG
	}
	}
}