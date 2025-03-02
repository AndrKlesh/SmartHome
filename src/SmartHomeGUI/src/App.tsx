import { useState } from "react";
import { BrowserRouter as Router, Route, Routes, Navigate } from "react-router-dom";
import Dashboard from "./components/Dashboard";
import Header from "./components/Header";
import MeasurementHistory from "./components/MeasurementHistory";
import Settings from "./components/Settings";
import "./components/styles.css";

function App() {
	const [isOpen, setIsOpen] = useState(false);

	return (
		<Router>
			<Header isOpen={isOpen} setIsOpen={setIsOpen} />
			<div className={`content ${isOpen ? "shifted" : ""}`}>
				<Routes>
					{/* Перенаправление на первый пункт меню при переходе на главную */}
					<Route path="/" element={<Navigate to="/dashboard/Общие" />} />
					<Route path="/dashboard/:name" element={<Dashboard />} />
					<Route path="/history/:topicName" element={<MeasurementHistory />} />
					<Route path="/settings" element={<Settings />} />
				</Routes>
			</div>
		</Router>
	);
}

export default App;
