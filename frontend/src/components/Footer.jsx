import './Footer.css';

const Footer = () => {
    return (
        <footer className="main-footer">
            <div className="footer-content">
                <div className="footer-links">
                    <a href="#" className="footer-link">О нас</a>
                    <a href="#" className="footer-link">Политика конфиденциальности</a>
                    <a href="#" className="footer-link">Условия использования</a>
                    <a href="#" className="footer-link">Контакты</a>
                </div>
                <div className="footer-copyright">
                    © 2024 AirlineAPI. Все права защищены.
                </div>
            </div>
        </footer>
    );
};

export default Footer;
