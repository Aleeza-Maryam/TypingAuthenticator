from flask import Flask, request, jsonify
import pickle
import numpy as np
import time
import os

app = Flask(__name__)

# Model load karo
model = pickle.load(open("models/typing_model.pkl", "rb"))
le = pickle.load(open("models/label_encoder.pkl", "rb"))

@app.route('/predict', methods=['POST'])
def predict():
    data = request.json
    
    features = [[
        data['total_time'],
        data['avg_pause'],
        data['max_pause'],
        data['min_pause'],
        data['typing_speed'],
        data['error_count'],
        data['error_rate'],
        data['rhythm_variance']
    ]]
    
    # Prediction karo
    prediction = model.predict(features)[0]
    probability = model.predict_proba(features)[0].max()
    
    predicted_user = le.inverse_transform([prediction])[0]
    
    # Claimed user se match karo
    claimed_user = data.get('claimed_username', '')
    is_authentic = (predicted_user == claimed_user) and (probability > 0.7)
    
    return jsonify({
        'predicted_user': predicted_user,
        'claimed_user': claimed_user,
        'confidence': round(float(probability) * 100, 2),
        'is_authentic': is_authentic,
        'message': '✅ Access Granted!' if is_authentic else '❌ Impostor Detected!'
    })

@app.route('/health', methods=['GET'])
def health():
    return jsonify({'status': 'running'})

if __name__ == '__main__':
    app.run(port=5000, debug=True)