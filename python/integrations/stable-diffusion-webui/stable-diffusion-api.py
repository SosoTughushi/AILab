import base64
import json
import requests
from typing import Dict, List, Optional, Union
from PIL import Image
from io import BytesIO
from typing import Optional

# ImageDomainModel class definition
class ImageDomainModel:
    def __init__(self, content_as_base64_string: str, width: int, height: int):
        self.content_as_base64_string = content_as_base64_string
        self.width = width
        self.height = height

class StableDiffusionApi:
    def __init__(self, api_url: str):
        self._api_url = api_url
        self._session = requests.Session()

    def image_to_image(self, request: "Img2ImgRequest") -> ImageDomainModel:
        options = {
            "property_naming_policy": "camel_case",
        }

        api_request = self.create_api_request_model(request)

        response = self._session.post(
            f"{self._api_url}/sdapi/v1/img2img",
            json=api_request,
            headers={"Content-Type": "application/json"},
        )

        response.raise_for_status()

        jsonResponse = response.json()

        img2ImgResponse = jsonResponse

        base64_string = img2ImgResponse["images"][0]
        image_bytes = base64.b64decode(base64_string)

        image = Image.open(BytesIO(image_bytes))

        return ImageDomainModel(base64_string, image.width, image.height)

    def text_to_image(self, request: "Text2ImgRequest") -> ImageDomainModel:
        options = {
            "property_naming_policy": "camel_case",
        }

        api_request = self.create_text2image_api_request_model(request)

        response = self._session.post(
            f"{self._api_url}/sdapi/v1/txt2img",
            json=api_request,
            headers={"Content-Type": "application/json"},
        )

        response.raise_for_status()

        jsonResponse = response.json()

        txt2ImgResponse = jsonResponse

        base64_string = txt2ImgResponse["images"][0]
        image_bytes = base64.b64decode(base64_string)

        image = Image.open(BytesIO(image_bytes))

        return ImageDomainModel(base64_string, image.width, image.height)

    def interrogate(self, req: "InterrogateRequest") -> str:
        request = {
            "image": req.input_image.content_as_base64_string,
            "model": "clip" if req.model == InterrogationModel.CLIP else "deepdanbooru",
        }

        options = {
            "property_naming_policy": "camel_case",
        }

        response = self._session.post(
            f"{self._api_url}/sdapi/v1/interrogate",
            json=request,
            headers={"Content-Type": "application/json"},
        )

        response.raise_for_status()

        jsonResponse = response.json()

        parsed_response = jsonResponse

        return parsed_response["caption"]

    def create_api_request_model(self, request: 'Img2ImgRequest') -> Dict:
        return {
            'init_images': [request.input_image.content_as_base64_string],
            'cfg_scale': request.cfg_scale,
            'denoising_strength': request.denoising_strength,
            'steps': request.steps,
            'width': request.input_image.width,
            'height': request.input_image.height,
            'seed': request.seed.value,
            'prompt': request.prompt,
            'negative_prompt': request.negative_prompt,
            'restore_faces': request.restore_faces,
            'sampler_name': 'Euler a',
            'resize_mode': 1
        }
    
    def create_text2image_api_request_model(self, request: 'Text2ImgRequest') -> Dict:
        return {
            'prompt': request.prompt,
            'width': request.width,
            'height': request.height,
            'steps': request.steps,
            'cfg_scale': request.cfg_scale,
            'seed': request.seed.value,
            'negative_prompt': request.negative_prompt,
            'restore_faces': request.restore_faces
        }

class Img2ImgRequest:
    def __init__(
        self,
        input_image: ImageDomainModel,
        prompt: str,
        denoising_strength: float,
        seed: int,
        cfg_scale: int = 20,
        steps: int = 20,
        negative_prompt: Optional[str] = None,
        restore_faces: bool = False,
    ):
        self.input_image = input_image
        self.prompt = prompt
        self.denoising_strength = denoising_strength
        self.seed = seed
        self.cfg_scale = cfg_scale
        self.steps = steps
        self.negative_prompt = negative_prompt
        self.restore_faces = restore_faces


class Text2ImgRequest:
    def __init__(
        self,
        prompt: str,
        seed: int,
        width: int,
        height: int,
        cfg_scale: float = 7,
        steps: int = 20,
        negative_prompt: Optional[str] = None,
        restore_faces: bool = False,
    ):
        self.prompt = prompt
        self.seed = seed
        self.width = width
        self.height = height
        self.cfg_scale = cfg_scale
        self.steps = steps
        self.negative_prompt = negative_prompt
        self.restore_faces = restore_faces

