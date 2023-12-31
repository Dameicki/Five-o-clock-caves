#version 440 core
out vec4 FragColor;

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos; 

struct DirLight {
    bool enable;

    vec3 direction;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    bool enable;

    vec3 position;  
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
	
    float constant;
    float linear;
    float quadratic;
}; 

struct SpotLight {
    bool enable;

	vec3 position;
	vec3 direction;
	float cutOff;
	float outerCutOff;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

struct Material {
    sampler2D texture_diffuse1;
    vec3      specular;
    vec3      color;
    vec3      fakeLight;
    float     shininess;
}; 

#define NR_POINT_LIGHTS 5
#define NR_SPOT_LIGHTS 7


uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform SpotLight spotLights[NR_SPOT_LIGHTS];
uniform DirLight dirLight;


uniform vec3 viewPos; 
uniform Material material;
#define FALSE_SPECULAR vec3(0.0)

// nagłówki funkcji
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);

void main()
{
	vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);

	// Efekt światła kierunkowego
	vec3 result = CalcDirLight(dirLight, norm, viewDir);

	// Efekt światła punktowego
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);  

	// Efekt światła refretorowego
    for(int i=0; i< NR_SPOT_LIGHTS; i++)
        result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);

    // Sztuczne doświetlenie postaci
    result += material.fakeLight * vec3(texture(material.texture_diffuse1, TexCoords));

    result *= material.color;
    result += (material.specular/10) * (1 / pow(dot(norm, viewDir),3));

    //result += material.specular * dot(norm, viewDir);

	FragColor = vec4(result, 1.0);
}

// calculates the color when using a directional light.
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    if(light.enable == true)
    {
        vec3 lightDir = normalize(-light.direction);

        // diffuse shading
        float diff = max(dot(normal, lightDir), 0.0);

        // specular shading
        vec3 reflectDir = reflect(-lightDir, normal);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

        // combine results
        vec3 ambient = light.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 ambient = light.ambient * vec3(1.0);
        vec3 diffuse = light.diffuse * diff * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 diffuse = light.diffuse * diff * vec3(1.0);
        vec3 specular = light.specular * spec * FALSE_SPECULAR;
        return (ambient + diffuse + specular);
    }
    if(light.enable == false)
    {
        return vec3(0.0);
    }
}

// calculates the color when using a point light.
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    if(light.enable == true)
    {
        vec3 lightDir = normalize(light.position - fragPos);

        // diffuse shading
        float diff = max(dot(normal, lightDir), 0.0);

        // specular shading
        vec3 reflectDir = reflect(-lightDir, normal);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);

        // attenuation
        float distance = length(light.position - fragPos);
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    

        // combine results
        vec3 ambient = light.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 ambient = light.ambient * vec3(1.0);
        vec3 diffuse = light.diffuse * diff * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 diffuse = light.diffuse * diff * vec3(1.0);
        vec3 specular = light.specular * spec * FALSE_SPECULAR;
        ambient *= attenuation;
        diffuse *= attenuation;
        specular *= attenuation;
        return (ambient + diffuse + specular);
    }
    if(light.enable == false)
    {
        return vec3(0.0);
    }
}

// calculates the color when using a spot light.
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    if(light.enable == true)
    {
        vec3 lightDir = normalize(light.position - fragPos);

        // diffuse shading
        float diff = max(dot(normal, lightDir), 0.0);

        // specular shading
        vec3 reflectDir = reflect(-lightDir, normal);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        
        // attenuation
        float distance = length(light.position - fragPos);
        float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));  

        // spotlight intensity
        float theta = dot(lightDir, normalize(-light.direction)); 
        float epsilon = light.cutOff - light.outerCutOff;
        float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
        
        // combine results
        vec3 ambient = light.ambient * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 ambient = light.ambient * vec3(1.0);
        vec3 diffuse = light.diffuse * diff * vec3(texture(material.texture_diffuse1, TexCoords));
        //vec3 diffuse = light.diffuse * diff * vec3(1.0);
        vec3 specular = light.specular * spec * FALSE_SPECULAR;
        ambient *= attenuation * intensity;
        diffuse *= attenuation * intensity;
        specular *= attenuation * intensity;
        return (ambient + diffuse + specular);
    }
    if(light.enable == false)
    {
        return vec3(0.0);
    }
}