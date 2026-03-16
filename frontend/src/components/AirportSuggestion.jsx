import './AirportSuggestion.css';

const AirportSuggestion = ({ airport, onClick }) => {
    return (
        <li className="airport-suggestion" onClick={() => onClick(airport)}>
            <div className="airport-name">{airport.name}</div>
            <div className="airport-location">{airport.location}</div>
        </li>
    );
};

export default AirportSuggestion;