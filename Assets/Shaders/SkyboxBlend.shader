Shader "RenderFX/SkyboxBlend" {

	Properties{
		_Tint("Tint Color", Color) = (.5, .5, .5, .5)
		_Tint1("Tint Color one", Color) = (.5, .5, .5, .5)
		_Tint2("Tint Color two", Color) = (.5, .5, .5, .5)
		_Blend1("Blend1", Range(0.0,1.0)) = 0.5
		_Blend2("Blend2", Range(0.0,1.0)) = 0.5
		_Blend3("Blend3", Range(0.0,1.0)) = 0.0
		_Skybox1("Skybox one", Cube) = ""
		_Skybox2("Skybox two", Cube) = ""
		_Skybox3("Skybox three", Cube) = ""
		_Skybox4("Skybox four", Cube) = ""
	}

		SubShader{
		Tags{ "Queue" = "Background" }
		Cull Off
		Fog{ Mode Off }
		Lighting Off
		Color[_Tint]
		Pass{
		SetTexture[_Skybox1]{ combine texture }
		SetTexture[_Skybox2]{ constantColor(0,0,0,[_Blend1]) combine texture lerp(constant) previous }
		SetTexture[_Skybox2]{ combine previous + -primary, previous * primary }

		SetTexture[_Skybox3]{ constantColor(0, 0, 0,[_Blend2]) combine texture lerp(constant) previous }

		SetTexture[_Skybox4]{ constantColor(0, 0, 0,[_Blend3]) combine texture lerp(constant) previous }
		}
	}

		Fallback "RenderFX/Skybox", 1
}
