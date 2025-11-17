from fastapi import FastAPI, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import requests
import json
import logging

# ---------- Logging ----------
logging.basicConfig(level=logging.INFO)

# ---------- FastAPI App ----------
app = FastAPI(title="Local Fitness Recommendation API")

# Allow frontend access
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # allow frontend from localhost or 127.0.0.1
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# ---------- Models ----------
class UserInput(BaseModel):
    Age: int
    Weight: float
    Goal: str
    FitnessLevel: str
    PreferredType: Optional[str] = None
    MaxDuration: Optional[int] = None


class Workout(BaseModel):
    Name: str
    Description: str
    FitnessLevel: str
    DurationMinutes: int
    Goal: str


class DietPlan(BaseModel):
    Calories: int
    Protein: int
    Carbs: int
    Fats: int
    Description: str


class Recommendation(BaseModel):
    Workouts: List[Workout]
    DietPlan: DietPlan


# ---------- Ollama Config ----------
OLLAMA_URL = "http://127.0.0.1:11434/api/chat"
MODEL_NAME = "phi3:mini"  # small, fast model for local use

@app.post("/api/recommend", response_model=Recommendation)
def get_recommendation(user_input: UserInput):
    logging.info(f"Received request for goal: {user_input.Goal}")

    prompt = f"""
You are a professional fitness trainer and nutritionist.
Based on this user's data:
Age: {user_input.Age}
Weight: {user_input.Weight}
Goal: {user_input.Goal}
Fitness Level: {user_input.FitnessLevel}
Preferred Type: {user_input.PreferredType or 'Any'}
Max Duration: {user_input.MaxDuration or 'Any'} minutes

Create a personalized workout and diet plan in valid JSON format:
{{
  "Workouts": [
    {{
      "Name": "string",
      "Description": "string",
      "FitnessLevel": "string",
      "DurationMinutes": int,
      "Goal": "string"
    }}
  ],
  "DietPlan": {{
    "Calories": int,
    "Protein": int,
    "Carbs": int,
    "Fats": int,
    "Description": "string"
  }}
}}
"""

    try:
        logging.info("Sending request to Ollama (phi3:mini)...")
        response = requests.post(
            OLLAMA_URL,
            json={
                "model": MODEL_NAME,
                "messages": [
                    {"role": "system", "content": "You are a helpful AI fitness assistant."},
                    {"role": "user", "content": prompt}
                ],
                "stream": False
            },
            timeout=60
        )

        if response.status_code != 200:
            logging.error(f"Ollama API returned {response.status_code}: {response.text}")
            raise HTTPException(status_code=500, detail="Ollama server returned an error. Check if it's running.")

        data = response.json()
        content = data.get("message", {}).get("content", "")

        if not content:
            raise ValueError("Empty response from phi3:mini model")

        # Extract JSON safely
        start = content.find("{")
        end = content.rfind("}") + 1
        if start == -1 or end == -1:
            raise ValueError("Invalid JSON format in model output")

        parsed_json = json.loads(content[start:end])
        logging.info("✅ Successfully parsed model output")
        return parsed_json

    except requests.exceptions.ConnectionError:
        logging.error("❌ Could not connect to Ollama. Make sure it's running locally.")
        raise HTTPException(status_code=500, detail="Cannot connect to Ollama. Run: ollama serve")
    except requests.exceptions.Timeout:
        logging.error("⏱ Request to Ollama timed out.")
        raise HTTPException(status_code=504, detail="Ollama model timed out. Try again.")
    except (ValueError, json.JSONDecodeError) as e:
        logging.error(f"❌ JSON parsing failed: {e}")
        raise HTTPException(status_code=500, detail=f"Error parsing model output: {e}")
