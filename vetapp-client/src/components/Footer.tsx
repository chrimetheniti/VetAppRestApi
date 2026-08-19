const Footer = () => {
    const currentYear: number = new Date().getFullYear()
    return (
        <footer className="bg-white text-gray-500 border-t border-gray-200 mt-16">
            <div className="container mx-auto py-6 text-center text-sm">
                &copy; {currentYear} VetApp. All Rights reserved.
            </div>
        </footer>
    )
}

export default Footer;