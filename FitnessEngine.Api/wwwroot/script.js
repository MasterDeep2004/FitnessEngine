const API_BASE = "http://127.0.0.1:8000"; // FastAPI backend URL

const promptInput = document.getElementById("prompt");
const getBtn = document.getElementById("getRecommendations");
const loadingText = document.getElementById("loadingText");
const errorText = document.getElementById("errorText");
const resultsDiv = document.getElementById("results");
const workoutsContainer = document.getElementById("workoutsContainer");
const dietContainer = document.getElementById("dietContainer");

getBtn.addEventListener("click", async () => {
    const prompt = promptInput.value.trim();
    if (!prompt) {
        errorText.textContent = "Please enter your fitness info.";
        return;
    }

    // Reset UI
    loadingText.style.display = "block";
    errorText.textContent = "";
    resultsDiv.style.display = "none";
    workoutsContainer.innerHTML = "";
    dietContainer.innerHTML = "";

    // Parse user input (very flexible)
    const parts = prompt.split(",");
    const input = {
        Age: 25,
        Weight: 70,
        FitnessLevel: parts[0]?.trim() || "Beginner",
        Goal: parts[1]?.trim() || "Muscle Gain",
        PreferredType: parts[2]?.trim() || "Any",
        MaxDuration: 45,
    };

    try {
        const res = await fetch(`${API_BASE}/api/recommend`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(input),
        });

        if (!res.ok) {
            throw new Error(`HTTP error ${res.status}`);
        }

        const data = await res.json();
        loadingText.style.display = "none";
        resultsDiv.style.display = "block";

        // Display workouts
        workoutsContainer.innerHTML = "";
        (data.Workouts || []).forEach((w) => {
            const card = document.createElement("div");
            card.className = "card";
            card.innerHTML = `
        <h4>${w.Name}</h4>
        <p>${w.Description}</p>
        <small>${w.FitnessLevel} | ${w.DurationMinutes} min | Goal: ${w.Goal}</small>
      `;
            workoutsContainer.appendChild(card);
        });

        // Display diet plan
        if (data.DietPlan) {
            const d = data.DietPlan;
            dietContainer.innerHTML = `
        <p><b>Calories:</b> ${d.Calories} kcal<br>
        <b>Protein:</b> ${d.Protein}g | <b>Carbs:</b> ${d.Carbs}g | <b>Fats:</b> ${d.Fats}g</p>
        <p>${d.Description}</p>
      `;
        }
    } catch (err) {
        console.error(err);
        loadingText.style.display = "none";
        errorText.textContent = "Error fetching recommendations. Make sure backend server is running.";
    }
});
