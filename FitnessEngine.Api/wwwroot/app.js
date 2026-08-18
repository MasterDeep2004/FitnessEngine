const form =
    document.getElementById("recommendationForm");

const loading =
    document.getElementById("loading");

const results =
    document.getElementById("results");

const workoutsContainer =
    document.getElementById("workouts");

const dietContainer =
    document.getElementById("diet");


form.addEventListener("submit", async function (event) {

    event.preventDefault();

    loading.classList.remove("hidden");

    results.classList.add("hidden");

    const data = {

        Age:
            Number(
                document.getElementById("age").value
            ),

        Weight:
            Number(
                document.getElementById("weight").value
            ),

        Goal:
            document.getElementById("goal").value,

        FitnessLevel:
            document.getElementById("fitnessLevel").value,

        PreferredType:
            document.getElementById("preferredType").value
            || null,

        MaxDuration:
            document.getElementById("maxDuration").value
                ? Number(
                    document.getElementById("maxDuration").value
                  )
                : null
    };

    try {

        const response =
            await fetch("/api/recommendation", {

                method: "POST",

                headers: {
                    "Content-Type":
                        "application/json"
                },

                body: JSON.stringify(data)
            });

        if (!response.ok) {

            const error =
                await response.json();

            throw new Error(
                error.message ||
                "Failed to generate recommendation."
            );
        }

        const recommendation =
            await response.json();

        displayRecommendation(
            recommendation
        );

    }
    catch (error) {

        alert(error.message);

    }
    finally {

        loading.classList.add("hidden");
    }
});


function displayRecommendation(
    recommendation
) {

    workoutsContainer.innerHTML = "";

    recommendation.workouts.forEach(
        workout => {

            const element =
                document.createElement("div");

            element.className = "workout";

            element.innerHTML = `
                <h3>${workout.name}</h3>

                <p>
                    ${workout.description}
                </p>

                <p>
                    <strong>Fitness Level:</strong>
                    ${workout.fitnessLevel}
                </p>

                <p>
                    <strong>Duration:</strong>
                    ${workout.durationMinutes} minutes
                </p>

                <p>
                    <strong>Goal:</strong>
                    ${workout.goal}
                </p>
            `;

            workoutsContainer.appendChild(
                element
            );
        }
    );


    const diet =
        recommendation.dietPlan;

    dietContainer.innerHTML = `

        <div class="diet-grid">

            <div class="diet-item">
                <strong>
                    ${diet.calories}
                </strong>

                <br>

                Calories
            </div>

            <div class="diet-item">
                <strong>
                    ${diet.protein}g
                </strong>

                <br>

                Protein
            </div>

            <div class="diet-item">
                <strong>
                    ${diet.carbs}g
                </strong>

                <br>

                Carbs
            </div>

            <div class="diet-item">
                <strong>
                    ${diet.fats}g
                </strong>

                <br>

                Fats
            </div>

        </div>

        <p>
            ${diet.description}
        </p>
    `;

    results.classList.remove("hidden");
}
