namespace StableDiffusionTools.Integrations.StableDiffusionWebUi;

public class Img2ImageApiResponse
{
    public string[] images { get; set; } = null!;

}

public class Text2ImageApiResponse
{
    public string[] images { get; set; } = null!;
}

public class Img2ImgApiRequest
{
    public List<string> Init_images { get; set; } = new List<string>();
    public int Resize_mode { get; set; } = 0;
    public double Denoising_strength { get; set; } = 0.75;
    public double Image_cfg_scale { get; set; } = 1.5;
    public string Mask { get; set; } = null;
    public int Mask_blur { get; set; } = 4;
    public int Inpainting_fill { get; set; } = 0;
    public bool Inpaint_full_res { get; set; } = true;
    public int Inpaint_full_res_padding { get; set; } = 0;
    public int Inpainting_mask_invert { get; set; } = 0;
    public int Initial_noise_multiplier { get; set; } = 1;
    public string Prompt { get; set; } = "";
    public List<string> Styles { get; set; } = new List<string>();
    public int Seed { get; set; } = -1;
    public int Subseed { get; set; } = -1;
    public int Subseed_strength { get; set; } = 0;
    public int Seed_resize_from_h { get; set; } = 0;
    public int Seed_resize_from_w { get; set; } = 0;
    public string Sampler_name { get; set; } = "Euler a";
    public int Batch_size { get; set; } = 1;
    public int N_iter { get; set; } = 1;
    public int? Steps { get; set; } = 20;
    public double Cfg_scale { get; set; } = 7.0;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public bool Restore_faces { get; set; } = false;
    public bool Tiling { get; set; } = false;
    public bool Do_not_save_samples { get; set; } = false;
    public bool Do_not_save_grid { get; set; } = false;
    public string? Negative_prompt { get; set; }
    public double Eta { get; set; } = 1.0;
    public int S_churn { get; set; } = 0;
    public int S_tmax { get; set; } = 0;
    public int S_tmin { get; set; } = 0;
    public int S_noise { get; set; } = 1;
    public Dictionary<string, object> Override_settings { get; set; } = new Dictionary<string, object>();
    public bool Override_settings_restore_afterwards { get; set; } = true;
    public string[] ScriptArgs { get; set; } = Array.Empty<string>();
    public int? Sampler_index { get; set; } = 0;
    public bool Include_init_images { get; set; } = false;
    public string Script_name { get; set; } = null;
    public bool Send_images { get; set; } = true;
    public bool Save_images { get; set; } = false;
    public string[] AlwaysonScripts { get; set; } = Array.Empty<string>();
}

public class Text2ImageApiRequest
{
    public bool Enable_hr { get; set; } = false;
    public double Denoising_strength { get; set; } = 0;
    public int Firstphase_width { get; set; } = 0;
    public int Firstphase_height { get; set; } = 0;
    public int Hr_scale { get; set; } = 2;
    public string Hr_upscaler { get; set; } = "string";
    public int Hr_second_pass_steps { get; set; } = 0;
    public int Hr_resize_x { get; set; } = 0;
    public int Hr_resize_y { get; set; } = 0;
    public string Prompt { get; set; } = "";
    public List<string> Styles { get; set; } = new List<string>();
    public int Seed { get; set; } = -1;
    public int Subseed { get; set; } = -1;
    public int Subseed_strength { get; set; } = 0;
    public int Seed_resize_from_h { get; set; } = -1;
    public int Seed_resize_from_w { get; set; } = -1;
    public string Sampler_name { get; set; } = "Euler a";
    public int Batch_size { get; set; } = 1;
    public int N_iter { get; set; } = 1;
    public int Steps { get; set; } = 50;
    public double Cfg_scale { get; set; } = 7;
    public int Width { get; set; } = 512;
    public int Height { get; set; } = 512;
    public bool Restore_faces { get; set; } = false;
    public bool Tiling { get; set; } = false;
    public bool Do_not_save_samples { get; set; } = false;
    public bool Do_not_save_grid { get; set; } = false;
    public string? Negative_prompt { get; set; }
    public double Eta { get; set; } = 0;
    public int S_churn { get; set; } = 0;
    public int S_tmax { get; set; } = 0;
    public int S_tmin { get; set; } = 0;
    public int S_noise { get; set; } = 1;
    public Dictionary<string, object> Override_settings { get; set; } = new Dictionary<string, object>();
    public bool Override_settings_restore_afterwards { get; set; } = true;
    public string[] ScriptArgs { get; set; } = Array.Empty<string>();
    public int? Sampler_index { get; set; } = 0;
    public string? Script_name { get; set; }
    public bool Send_images { get; set; } = true;
    public bool Save_images { get; set; } = false;
    public Dictionary<string, object> AlwaysonScripts { get; set; } = new Dictionary<string, object>();
}

public record InterrogateApiRequest(string Image, string Model);